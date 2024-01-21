using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;

namespace xjjxmm.mqtt.Channel;

internal class MqttChannel : IDisposable
{
    private readonly SocketProxy _socketClient = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();
   
    //private readonly Dictionary<PacketType, Queue<ReceivedPacket>> _commands = new ();
    
    public Func<ReceivedPacket, Task>?  Received { get; set; }
    
    public async Task Connect(ConnectPacket packet)
    {
        await _socketClient.Connect(packet.Host, packet.Port);
        Receive();
    }
    
    public async Task Send(ArraySegment<byte> bytes)
    {
        await _socketClient.Send(bytes, _cancellationTokenSource.Token);
    }
    
     private async Task Receive()
    {
        while (true)
        {
            var receivePacket = new ReceivedPacket(_socketClient);
            var size = await receivePacket.Receive(); 
            if (size > 0)
            {
                if (Received != null)
                {
                   Received.Invoke(receivePacket);
                }
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