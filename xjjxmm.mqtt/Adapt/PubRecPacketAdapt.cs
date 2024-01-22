using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

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

    private List<byte> Data { get; } = new List<byte>();
    
    private readonly byte lsb;
    private readonly byte msb;
    protected  void PushHeaders()
    {
        byte header = (byte)PacketType.PubRec << 4;

        Data.Add(header);
    }

    protected  void PushRemainingLength()
    {
        Data.Add(0x02);
    }

    protected  void PushVariableHeader()
    {
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
        return new PubRecOption()
        {
            //PacketIdentifier = packet.PacketIdentifier
        };
    }
}