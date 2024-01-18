using System.Collections.Concurrent;
using mqtt.server.Options;
using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Service;

namespace xjjxmm.mqtt.Net;

interface IHandel
{
  
    Task<IOption?> Handel(Command.Command command);
}

internal class SendConnectHandel : IHandel
{
 
    public Task<IOption> Handel(Command.Command command)
    {
        if (command.Name == CommandEnum.SendConnect)
        {
            var service = new ConnectService();
            await service.Send(command.Option);
        }
        else if (command.Name == CommandEnum.ReceiveConnect)
        {
            
        }

        return null;
    }
}

internal class MqttPacketDispatcher
{
    private readonly List<IHandel> _handels = new();
    private ConcurrentQueue<ICommand> _commands = new();
    private ConcurrentQueue<ICommand> _tmpCommands = new();

    private static readonly MqttPacketDispatcher _instance = new MqttPacketDispatcher();
    public MqttPacketDispatcher Instance => _instance;

    public void RegistHandel(IHandel handel)
    {
        _handels.Add(handel);
    }
    
    public void Dispatch<IPacket> (Command.Command command)
    {
        foreach (var handel in _handels)
        {
            var packet = await handel.Handel(handel);
            
        }
    }
    
    public void AddCommand(ICommand packet)
    {
        _commands.Enqueue(packet);
    }

    public async Task<bool> Dispatch(ReceivedPacket packet)
    {
        var exists = false;
            ICommand command;
            while (_commands.TryDequeue(out command))
            {
                if (await command.IsHandel(packet))
                {
                    exists = true;
                    await command.SetResult(packet);
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

            return exists;
    }
}