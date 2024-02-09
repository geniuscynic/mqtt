using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;
using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.Adapt;

internal class PubRelPacketAdapt : IAdaptFactory
{
    private readonly PubRelPacket packet;
    public PubRelPacketAdapt(PubRelOption option, ushort packetIdentifier)
    {
        packet = new PubRelPacket
        {
            PacketIdentifier = packetIdentifier
        };

    }
    
    public PubRelPacketAdapt(ReceivedPacket received)
    {
        //received.Dump();
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new PubRelPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public PubRelPacketAdapt(PubRelPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

 
  
    public ArraySegment<byte> Encode()
    {
        var packetType = (byte)ControlPacketType.PubRel << 4;

        var writeHelper = new BufferWriteHelper();
        writeHelper.SetHeader((byte)packetType);
        writeHelper.AddPacketIdentifier(packet.PacketIdentifier);
        
        return writeHelper.Build();
    }

    public IOption GetOption()
    {
        return new PubRelOption()
        {
           // PacketIdentifier = packet.PacketIdentifier
        };
    }
}