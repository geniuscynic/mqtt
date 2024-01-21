using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Server;

internal class MqttServer //: IDisposable
{
    /*private readonly SocketProxy _socketClient = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();
   
    private readonly Queue<ICommand> _commands = new Queue<ICommand>();
    Queue<ICommand> _tmpCommands = new Queue<ICommand>();
    //创建与远程主机的连接
    
    public async Task Start()
    {
        await ClientConnected();
    }
        
    private async Task ClientConnected()
    {
        while (true)
        {
            var client = await _server.Accept();
            SocketInfo socketInfo = new SocketInfo();
            _socketId++;
            socketInfo.Id = _socketId;
            socketInfo.Socket = client;
            ReceiveMessage(socketInfo);
        }
    }
    
    public async Task<ReceivedPacket> Send(ISendCommand command, int timeout = 1000 * 10)
    {
        _commands.Enqueue(command);
        await _socketClient.Send(command.Encode(), _cancellationTokenSource.Token);
        
        using var cts = new CancellationTokenSource();
        cts.Token.Register(() => command.Result.TrySetException(new TimeoutException()), useSynchronizationContext: false);
        cts.CancelAfter(timeout);
        
        return await command.Result.Task;
    }
    
    public async Task SendNoAnswer(ISendCommand command)
    {
        await _socketClient.Send(command.Encode(), _cancellationTokenSource.Token);
    }
    
    public async Task<ReceivedPacket> ReceiveCommand(ICommand command)
    {
        _commands.Enqueue(command);
        
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
                        Task.Run(()=>
                        {
                            command.Result.SetResult(receivePacket);
                        });
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
     }*/
}