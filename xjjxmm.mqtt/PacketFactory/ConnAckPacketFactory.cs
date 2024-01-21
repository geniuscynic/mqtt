using mqtt.server.Options;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class ConnAckPacketFactory : IPacketFactory
{
    private readonly ConnAckPacket packet;
    public ConnAckPacketFactory(ConnAckOption option)
    {
       
    }
    
    public ConnAckPacketFactory(ReceivedPacket received)
    {
        packet = new();
        var readerHelper = received.GetPacketHelper();
        packet.IsSessionPresent = readerHelper.Next() == 1;
        packet.ReasonCode = readerHelper.Next();
    }
    
    public ConnAckPacketFactory(ConnAckPacket option)
    {
        this.packet = option;
    }
    public MqttPacket GetPacket()
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