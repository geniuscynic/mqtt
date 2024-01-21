using mqtt.server.Options;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.PacketFactory;

internal class UnSubscribePacketFactory : IPacketFactory
{
    private readonly UnSubscribePacket packet;
    public UnSubscribePacketFactory(UnSubscribeOption option)
    {
        packet = new UnSubscribePacket()
        {
            TopicFilters = option.TopicFilters
        };

    }
    
    public UnSubscribePacketFactory(ReceivedPacket received)
    {
        /*var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new UnSubscribePacket
        {
            TopicFilters = option.TopicFilters
        };*/
    }
    
    public UnSubscribePacketFactory(UnSubscribePacket option)
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
        byte header = (byte)PacketType.UnSubscribe << 4;

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
        return new UnSubscribeOption()
        {
            TopicFilters = packet.TopicFilters
        };
    }
}