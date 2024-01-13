using mqtt.server.Constant;
using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Channel;

internal class MqttClientChannel3 : IDisposable
{
    private readonly SocketProxy _socketClient = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();
   
    private readonly Dictionary<byte, Queue<PacketType>> _commands = new ();
  
    
    public async Task Connect(ConnectOption option)
    {
        await _socketClient.Connect(option.Host, option.Port);
        Receive();
    }
    
    public async Task Send(Packet packet)
    {
        await _socketClient.Send(packet.NextAll(), _cancellationTokenSource.Token);
    }
    
    public async Task<ReceivedPacket> ReceiveCommand(ICommand command)
    {
        _commands.Enqueue(command);
        
        return await command.Result.Task;
    }
    
     private async Task Receive()
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
                if (!_commands.ContainsKey(commandName))
                {
                    _commands.Add(commandName, new Queue<ReceivedPacket>());
                }
                _commands[commandName].Enqueue(receivePacket);
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