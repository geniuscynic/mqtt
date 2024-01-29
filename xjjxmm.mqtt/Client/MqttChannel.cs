using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Client;

internal class MqttChannel : IDisposable
{
    private readonly SocketProxy _socketClient;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Func<ReceivedPacket, Task> _received;
    
    public MqttChannel(SocketProxy socketClient, Func<ReceivedPacket, Task>  received)
    {
        _socketClient = socketClient;
        _received = received;
    }
    
    public async Task Connect(ConnectPacket packet)
    {
        await _socketClient.Connect(packet.Host, packet.Port);
    }
    
    public async Task Send(ArraySegment<byte> bytes)
    {
        await _socketClient.Send(bytes, _cancellationTokenSource.Token);
    }
    
    public async Task Receive()
    {
        while (true)
        {
            var receivePacket = new ReceivedPacket(_socketClient);
            var size = await receivePacket.Receive(); 
            if (size > 0)
            {
                
                  _received.Invoke(receivePacket);
                
            }
            else
            {
                break;
            }
        }
    }

     public void Dispose()
     {
         _socketClient.Close();
     }
}