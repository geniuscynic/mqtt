using mqtt.server.Options;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PubAckPacketFactory : IPacketFactory
{
    private readonly PubAckPacket packet;
    public PubAckPacketFactory(PubAckOption option)
    {
        packet = new PubAckPacket
        {
            PacketIdentifier = option.PacketIdentifier
        };

    }
    
    public PubAckPacketFactory(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new PubAckPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public PubAckPacketFactory(PubAckPacket option)
    {
        this.packet = option;
    }
    
    public MqttPacket GetPacket()
    {
        return packet;
    }

    private List<byte> Data { get; } = new List<byte>();
    
    private readonly byte lsb;
    private readonly byte msb;
    protected  void PushHeaders()
    {
        byte header = (byte)PacketType.PubAck << 4;

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
        return new PubAckOption
        {
            PacketIdentifier = packet.PacketIdentifier
        };
    }
}