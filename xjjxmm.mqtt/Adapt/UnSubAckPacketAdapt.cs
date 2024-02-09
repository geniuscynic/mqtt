using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;
using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.Adapt;

internal class UnSubAckPacketAdapt : IAdaptFactory
{
    private readonly UnSubAckPacket packet;
    public UnSubAckPacketAdapt(UnSubAckOption option, ushort packetIdentifier)
    {
        packet = new UnSubAckPacket()
        {
            PacketIdentifier = packetIdentifier
        };

    }
    
    public UnSubAckPacketAdapt(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new UnSubAckPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public UnSubAckPacketAdapt(UnSubAckPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }
    
    
    public ArraySegment<byte> Encode()
    {
       
        var packetType = (byte)ControlPacketType.UnSubAck << 4;

        var writeHelper = new BufferWriteHelper();
        writeHelper.SetHeader((byte)packetType);
        writeHelper.AddPacketIdentifier(packet.PacketIdentifier);
       

        return writeHelper.Build();
    }

    public IOption GetOption()
    {
        return new UnSubAckOption()
        {
           // PacketIdentifier = packet.PacketIdentifier
        };
    }
}