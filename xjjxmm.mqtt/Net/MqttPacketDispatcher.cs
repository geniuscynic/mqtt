using System.Collections.Concurrent;
using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.MqttPacket;

namespace xjjxmm.mqtt.Net;

internal class MqttPacketDispatcher
{
    private ConcurrentQueue<ICommand> _commands = new();
    private ConcurrentQueue<ICommand> _tmpCommands = new();

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