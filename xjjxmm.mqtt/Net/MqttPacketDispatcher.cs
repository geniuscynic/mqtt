using System.Collections.Concurrent;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;

namespace xjjxmm.mqtt.Net;

internal class AwaitableMqttPacket(PacketType packet)
{
    public PacketType PacketType { get; } = packet;

    private TaskCompletionSource<ReceivedPacket> _result  = new (TaskCreationOptions.RunContinuationsAsynchronously);

    public async Task<ReceivedPacket> GetResult()
    {
        return await _result.Task;
    }

    public void SetResult(ReceivedPacket receivedPacket)
    {
        _result.SetResult(receivedPacket);
    }
    
}

internal class Dispatcher
{
    private Dispatcher() {}

    public static Dispatcher Instance { get; } = new ();


    private ConcurrentQueue<AwaitableMqttPacket> _commands = new();
    private ConcurrentQueue<AwaitableMqttPacket> _tmpCommands = new();
    public async Task<IPacketFactory?> AddEventHandel(PacketType packetType)
    {
        AwaitableMqttPacket packet = new AwaitableMqttPacket(packetType);
        _commands.Enqueue(packet);
 
        var receivePacket =  await packet.GetResult();
        return PacketFactory.PacketFactory.CreatePacketFactory(receivePacket);
    }
    
    public void Dispatch(ReceivedPacket packet)
    {
        AwaitableMqttPacket command;
        while (_commands.TryDequeue(out command))
        {
            if (command.PacketType == packet.GetPacketType())
            {
                command.SetResult(packet);
                break;
            }
            else
            {
                _tmpCommands.Enqueue(command);
            }
        }

        while (_tmpCommands.TryDequeue(out command))
        {
            _commands.Enqueue(command);
        }
    }
}