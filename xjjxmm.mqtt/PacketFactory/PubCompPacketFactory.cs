using mqtt.server.Options;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;

namespace xjjxmm.mqtt.PacketFactory;

internal class PubCompPacketFactory : IPacketFactory
{
    private readonly PubCompPacket packet;
    public PubCompPacketFactory(PubCompOption option, ushort packetIdentifier)
    {
        packet = new PubCompPacket
        {
            PacketIdentifier = packetIdentifier
        };

    }
    
    public PubCompPacketFactory(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new PubCompPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public PubCompPacketFactory(PubCompPacket option)
    {
        this.packet = option;
    }
    
    public MqttPacket.MqttPacket GetPacket()
    {
        return packet;
    }

    private List<byte> Data { get; } = new List<byte>();
    
    private readonly byte lsb;
    private readonly byte msb;
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