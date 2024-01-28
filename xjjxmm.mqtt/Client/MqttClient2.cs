using mqtt.server.Constant;
using xjjxmm.mqtt.Adapt;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Client;

public class MqttClient2 : IDisposable
{
    private readonly MqttClientChannel _mqttChannel;
    private readonly Dispatcher _dispatcher;
    public Func<ReceiveOption, Task>? ReceiveMessage { get; set; }
   // private readonly HashSet<int> _packetIdentifierHashSet = new();
    private readonly PacketIdentifierProvider _packetIdentifierProvider = new();

    public MqttClient2()
    {
        SocketProxy socketClient = new SocketProxy();
        _mqttChannel = new MqttClientChannel(socketClient);
       
    }

    public async Task<ConnAckOption> Connect(ConnectOption option)
    {
        _mqttChannel.ReceiveMessage = Receive;
        
        return await _mqttChannel.Connect(option);
    }


    private async Task Receive(Packet.MqttPacket receivedPacket)
    {
        var packet = (PublishPacket)receivedPacket;
        await ReceiveMessage?.Invoke(new ReceiveOption(packet.TopicName, packet.Message));
    }
    
   

    public async Task Publish(PublishOption option)
    {
        await _mqttChannel.Publish(option);
    }

 

    public async Task<SubAckOption> Subscribe(SubscribeOption option)
    {
        return await _mqttChannel.Subscribe(option);
    }

    public void Dispose()
    {
        _mqttChannel.Dispose();
    }
}