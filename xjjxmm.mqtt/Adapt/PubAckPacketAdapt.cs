using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;
using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.Adapt;

internal class PubAckPacketAdapt : IAdaptFactory
{
    private readonly PubAckPacket packet;
    public PubAckPacketAdapt(PubAckOption option, ushort packetIdentifier)
    {
        packet = new PubAckPacket
        {
            PacketIdentifier = packetIdentifier
        };

    }
    
    public PubAckPacketAdapt(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new PubAckPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public PubAckPacketAdapt(PubAckPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    public ArraySegment<byte> Encode()
    {
        var packetType = (byte)ControlPacketType.PubAck << 4;

        var writeHelper = new BufferWriteHelper();
        writeHelper.SetHeader((byte)packetType);
        writeHelper.AddPacketIdentifier(packet.PacketIdentifier);
        
        return writeHelper.Build();
    }

    public IOption GetOption()
    {
        return new PubAckOption
        {
           // PacketIdentifier = packet.PacketIdentifier
        };
    }
}