using mqtt.client.test;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;
using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.Adapt;

internal class PubCompPacketAdapt : IAdaptFactory
{
    private readonly PubCompPacket packet;
    public PubCompPacketAdapt(PubCompOption option, ushort packetIdentifier)
    {
        packet = new PubCompPacket
        {
            PacketIdentifier = packetIdentifier
        };

    }
    
    public PubCompPacketAdapt(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new PubCompPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public PubCompPacketAdapt(PubCompPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }
    
    public ArraySegment<byte> Encode()
    {
        var packetType = (byte)PacketType.PubComp << 4;

        var writeHelper = new BufferWriteHelper();
        writeHelper.SetHeader((byte)packetType);
        writeHelper.AddPacketIdentifier(packet.PacketIdentifier);
        
        writeHelper.Build().Dump("suback");
        
        return writeHelper.Build();
    }

    public IOption GetOption()
    {
        return new PubCompOption()
        {
          // PacketIdentifier = packet.PacketIdentifier
        };
    }
}