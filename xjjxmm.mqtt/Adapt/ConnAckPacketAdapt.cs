using mqtt.client.test;
using xjjxmm.mqtt.Constant;
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
        packet.ReasonCode = (ConnectReturnCode)readerHelper.Next();
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
        byte header = (byte)ControlPacketType.ConnAck << 4;
        byte remainingLength = 0x02;
        var variableHeader = new byte[]
        {
            0x00,
            0x00
        };
       var bytes = new []
       {
           header,
           remainingLength,
           variableHeader[0],
           variableHeader[1]
       };
       // new ArraySegment<byte>(bytes).Dump("conack");
       return bytes;
    }

    public IOption GetOption()
    {
        return new ConnAckOption();
    }
}