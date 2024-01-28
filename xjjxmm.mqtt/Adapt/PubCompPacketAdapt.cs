using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

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

    private List<byte> Data { get; } = new List<byte>();
    
   
    protected  void PushHeaders()
    {
        byte header = (byte)PacketType.PubRel << 4;

        Data.Add(header);
    }

    protected  void PushRemainingLength()
    {
        Data.Add(0x02);
    }

    protected  void PushVariableHeader()
    {
        var msb = (byte)(packet.PacketIdentifier >> 8);
        var lsb = (byte)(packet.PacketIdentifier & 255);
        Data.Add(msb);
        Data.Add(lsb);
    }

    protected  void PushPayload()
    {
    }

    
    public ArraySegment<byte> Encode()
    {
        PushHeaders();
        PushRemainingLength();
        PushVariableHeader();
        PushPayload();
        return Data.ToArray();
    }

    public IOption GetOption()
    {
        return new PubCompOption()
        {
           // PacketIdentifier = packet.PacketIdentifier
        };
    }
}