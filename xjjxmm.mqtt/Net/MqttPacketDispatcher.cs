using System.Collections.Concurrent;
using xjjxmm.mqtt.Adapt;
using xjjxmm.mqtt.Client;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Net;

internal class AwaitableMqttPacket(PacketType packet, ushort packetIdentifier )
{
    public ushort PacketIdentifier { get; } = packetIdentifier;
    public PacketType PacketType { get; } = packet;

    private TaskCompletionSource<IAdaptFactory> _result  = new (TaskCreationOptions.RunContinuationsAsynchronously);

    public async Task<IAdaptFactory> GetResult()
    {
        return await _result.Task;
    }

    public void SetResult(IAdaptFactory receivedPacket)
    {
        _result.SetResult(receivedPacket);
    }
    
}

internal class Dispatcher(MqttChannel mqttChannel)
{
    private ConcurrentQueue<AwaitableMqttPacket> _commands = new();
    private ConcurrentQueue<AwaitableMqttPacket> _tmpCommands = new();
    public async Task<IAdaptFactory?> AddEventHandel(IAdaptFactory packetFactory, PacketType packetType)
    {
        var packet = packetFactory.GetPacket();
        ushort packetIdentifier = 0;
        if (packet is IdentifierPacket identifierPacket)
        {
            packetIdentifier = identifierPacket.PacketIdentifier;
        }
        
        AwaitableMqttPacket awaitableMqttPacket = new AwaitableMqttPacket(packetType, packetIdentifier);
        _commands.Enqueue(awaitableMqttPacket);

        await mqttChannel.Send(packetFactory.Encode());
        
        var receivePacket =  await awaitableMqttPacket.GetResult();
        return receivePacket;
    }
    
    public void Dispatch(ReceivedPacket packet)
    {
        AwaitableMqttPacket command;
        while (_commands.TryDequeue(out command))
        {
            if (command.PacketType == packet.GetPacketType())
            {
                var packetFactory =  Adapt.AdaptFactory.CreatePacketFactory(packet);
                var mqttPacket = packetFactory!.GetPacket();
                if (mqttPacket is IdentifierPacket identifierPacket)
                {
                    if (identifierPacket.PacketIdentifier == command.PacketIdentifier)
                    {
                        command.SetResult(packetFactory);
                        break;     
                    }
                    else
                    {
                        _tmpCommands.Enqueue(command);
                    }
                }
                else
                {
                    command.SetResult(packetFactory);
                    break;
                }
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