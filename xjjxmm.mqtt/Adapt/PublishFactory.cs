using mqtt.client.test;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class PublishPacketFactory : IAdaptFactory
{
    private readonly PublishPacket packet;
    public PublishPacketFactory(PublishOption option, ushort packetIdentifier)
    {
        packet = new PublishPacket
        {
            TopicName = option.TopicName,
            Message = option.Message,
            Dup = option.Dup,
            QoS = option.QoS,
            Retain = option.Retain,
            PacketIdentifier = packetIdentifier
        };

    }
    
    public PublishPacketFactory(ReceivedPacket received)
    {
        
        var helper = received.GetPacketHelper();
        var remainingLength = _buffer.RemainingLength;

        var retain = (_buffer.Header & 0x01) == 0x01;
        var qos = (_buffer.Header & 0x06) >> 1;
        var dup = (_buffer.Header & 0x08) == 0x08;

        var topicLength = helper.NextTwoByteInt();
        var topic = helper.NextStr(topicLength);

        var msgLength = remainingLength - topicLength - 2;
        if (qos > 0)
        {
            var packetIdentifier = helper.NextTwoByteInt(); //只有当QoS等级是1或2时，报文标识符(Packet Identifier)字段才能出现在PUBLISH报文中
            packet.PacketIdentifier = packetIdentifier;

            msgLength -= 2;
        }

        var msg = helper.NextStr(msgLength);


        packet.TopicName = topic;
        packet.Message = msg;
        packet.Retain = retain;
        packet.QoS = (byte)qos;
        packet.Dup = dup;
        
    }
    
    public PublishPacketFactory(PublishPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    private List<byte> Data { get; } = new List<byte>();
    
    private ReceivedPacket _buffer;
    private byte[] _msgByte;

    private byte[] _subjectByte;
  

    protected void PushHeaders()
    {
        var header = (byte)PacketType.Publish << 4;

        if (packet.Dup) header |= 0x01 << 3;

        header |= packet.QoS << 1;

        if (packet.Retain) header |= 0x01;

        Data.Add(Convert.ToByte(header));
    }

    protected void PushRemainingLength()
    {
        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        var len = 2 + _subjectByte.Length + _msgByte.Length;
        if (packet.QoS > 0) len += 2;

        foreach (var l in UtilHelpers.ComputeRemainingLength(len)) Data.Add(Convert.ToByte(l));
    }

    protected void PushVariableHeader()
    {
        Data.Add((byte)(_subjectByte.Length >> 8));
        Data.Add((byte)(_subjectByte.Length & 255));
        Data.AddRange(_subjectByte);

        // 报文标识符 Packet Identifier
        if (packet.PacketIdentifier > 0)
        {
            var msb = (byte)(packet.PacketIdentifier >> 8);
            var lsb = (byte)(packet.PacketIdentifier & 255);
            Data.Add(msb);
            Data.Add(lsb);
        }
    }


    protected void PushPayload()
    {
        Data.AddRange(_msgByte);
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
        return new PingRespOption();
    }
}