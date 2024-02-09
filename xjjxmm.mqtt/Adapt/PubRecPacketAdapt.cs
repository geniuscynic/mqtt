using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;
using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.Adapt;

internal class PubRecPacketAdapt : IAdaptFactory
{
    private readonly PubRecPacket packet;
    public PubRecPacketAdapt(PubRecOption option, ushort packetIdentifier)
    {
        packet = new PubRecPacket
        {
            PacketIdentifier = packetIdentifier
        };

    }
    
    public PubRecPacketAdapt(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new PubRecPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public PubRecPacketAdapt(PubRecPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    public ArraySegment<byte> Encode()
    {
        var packetType = (byte)ControlPacketType.PubRec << 4;

        var writeHelper = new BufferWriteHelper();
        writeHelper.SetHeader((byte)packetType);
        writeHelper.AddPacketIdentifier(packet.PacketIdentifier);
        
        return writeHelper.Build();
    }

    public IOption GetOption()
    {
        return new PubRecOption()
        {
            //PacketIdentifier = packet.PacketIdentifier
        };
    }
}