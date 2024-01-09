using mqtt.server;
using mqtt.server.Options;
using mqtt.server.Packet;
using mqtt.server.Util;
using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Channel;

internal class MqttChannel2 : IDisposable
{
    private readonly SocketClient _socketClient = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();
   
    private readonly Queue<ICommand> _commands = new Queue<ICommand>();
    Queue<ICommand> _tmpCommands = new Queue<ICommand>();
    //创建与远程主机的连接
    
    public async Task Connect(ConnectOption option)
    {
        await _socketClient.Connect(option.Host, option.Port);
    }
    
    public async Task<ReceivedPacket> Send(ICommand command, int timeout = 1000 * 1)
    {
        _commands.Enqueue(command);
        await _socketClient.Send(command.Encode(), _cancellationTokenSource.Token);
        
        using var cts = new CancellationTokenSource();
        cts.Token.Register(() => command.Result.TrySetException(new TimeoutException()), useSynchronizationContext: false);
        cts.CancelAfter(timeout);
        
        return await command.Result.Task;
    }
    
     public async Task Receive()
    {
        const int bufferSize = 2;
        ArraySegment<byte> buffer = new (new byte[bufferSize]);

        while (true)
        {
            var size = await _socketClient.Receive(buffer); 
            if (size > 0)
            {
                var remainingLength = buffer[1];
                
                var body = await _socketClient.Receive(remainingLength);
                var receivePacket = new ReceivedPacket(buffer[0], remainingLength, body);
                if (receivePacket.RemainingLength > remainingLength)
                {
                    body = await _socketClient.Receive(receivePacket.TotalLength - remainingLength);
                    receivePacket.Append(body);
                }

               
                var commandName = receivePacket.GetCommandName();
                while (_commands.Any())
                {
                    var command = _commands.Dequeue();
                    if (command.AcceptCommand == commandName )
                    {
                        command.Result.SetResult(receivePacket);
                        break;
                    }
                    else
                    {
                        _tmpCommands.Enqueue(command);
                    }
                }

                while (_tmpCommands.Any())
                {
                    _commands.Enqueue(_tmpCommands.Dequeue());
                }
            }
            else
            {
                //Console.WriteLine($"{socketInfo.Id} 断开连接");
                break;
            }
        }
    }

     public void Dispose()
     {
         _socketClient.Close();
     }
}