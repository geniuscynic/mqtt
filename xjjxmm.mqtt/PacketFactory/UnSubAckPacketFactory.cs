using mqtt.server.Options;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;

namespace xjjxmm.mqtt.PacketFactory;

internal class UnSubAckPacketFactory : IPacketFactory
{
    private readonly UnSubAckPacket packet;
    public UnSubAckPacketFactory(UnSubAckOption option)
    {
        packet = new UnSubAckPacket()
        {
            PacketIdentifier = option.PacketIdentifier
        };

    }
    
    public UnSubAckPacketFactory(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new UnSubAckPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public UnSubAckPacketFactory(UnSubAckPacket option)
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
        byte header = (byte)PacketType.UnSubAck << 4;

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
        return new UnSubAckOption()
        {
            PacketIdentifier = packet.PacketIdentifier
        };
    }
}