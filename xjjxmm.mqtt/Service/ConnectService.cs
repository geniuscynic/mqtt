using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Service;

internal class ConnectService
{
    private readonly MqttChannel3 _mqttChannel;
   
    public ConnectService(MqttChannel3 mqttChannel)
    {
        _mqttChannel = mqttChannel;
    }
    
    public async Task Send(ConnectOption option)
    {
        var packet = ConvertTo(option);
        var bytes = ConvertTo(packet);
        await _mqttChannel.Connect(packet);
        await _mqttChannel.Send(bytes);
    }

    public bool IsHandel(ReceivedPacket receivedPacket)
    {
        return receivedPacket.GetPacketType() == PacketType.Connect;
    }
    
    public ConnectPacket GetResult(ReceivedPacket packet)
    {
        return new ConnectPacket(); 
    }
    
    private ConnectPacket ConvertTo(ConnectOption option)
    {
        return new ConnectPacket();
    }
    
    private ArraySegment<byte> ConvertTo(ConnectPacket packet)
    {
        return new ArraySegment<byte>();
    }
}