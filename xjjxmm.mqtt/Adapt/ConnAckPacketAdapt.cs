using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class ConnAckPacketAdapt : IAdaptFactory
{
    private readonly ConnAckPacket packet;
    public ConnAckPacketAdapt(ConnAckOption option)
    {
       
    }
    
    public ConnAckPacketAdapt(ReceivedPacket received)
    {
        packet = new();
        var readerHelper = received.GetPacketHelper();
        packet.IsSessionPresent = readerHelper.Next() == 1;
        packet.ReasonCode = readerHelper.Next();
    }
    
    public ConnAckPacketAdapt(ConnAckPacket option)
    {
        this.packet = option;
    }
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    public ArraySegment<byte> Encode()
    {
        throw new NotImplementedException();
    }

    public IOption GetOption()
    {
        return new ConnAckOption();
    }
}