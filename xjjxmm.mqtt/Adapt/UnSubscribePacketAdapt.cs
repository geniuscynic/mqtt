using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class UnSubscribePacketAdapt : IAdaptFactory
{
    private readonly UnSubscribePacket packet;
    public UnSubscribePacketAdapt(UnSubscribeOption option)
    {
        packet = new UnSubscribePacket()
        {
            TopicFilters = option.TopicFilters
        };

    }
    
    public UnSubscribePacketAdapt(ReceivedPacket received)
    {
        /*var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new UnSubscribePacket
        {
            TopicFilters = option.TopicFilters
        };*/
    }
    
    public UnSubscribePacketAdapt(UnSubscribePacket option)
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