using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PublishPacket : AbstractDataPacket<PublishOption>
{
    private readonly ReceivedPacket _buffer;
    private readonly byte[] _msgByte;

    private readonly byte[] _subjectByte;
    private readonly PublishOption publishOption;

    public PublishPacket(PublishOption publishOption)
    {
        this.publishOption = publishOption;
        _subjectByte = publishOption.TopicName.ToBytes();
        _msgByte = publishOption.Message.ToBytes();
    }

    public PublishPacket(ReceivedPacket buffer)
    {
        _buffer = buffer;
    }

    protected override void PushHeaders()
    {
        var header = PacketType.PUBLISH << 4;

        if (publishOption.Dup) header |= 0x01 << 3;

        header |= publishOption.QoS << 1;

        if (publishOption.Retain) header |= 0x01;

        Data.Add(Convert.ToByte(header));
    }

    protected override void PushRemainingLength()
    {
        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        var len = 2 + _subjectByte.Length + _msgByte.Length;
        if (publishOption.QoS > 0) len += 2;

        foreach (var l in UtilHelpers.ComputeRemainingLength(len)) Data.Add(Convert.ToByte(l));
    }

    protected override void PushVariableHeader()
    {
        Data.Add((byte)(_subjectByte.Length >> 8));
        Data.Add((byte)(_subjectByte.Length & 255));
        Data.AddRange(_subjectByte);

        // 报文标识符 Packet Identifier
        if (publishOption.PacketIdentifier > 0)
        {
            var msb = (byte)(publishOption.PacketIdentifier >> 8);
            var lsb = (byte)(publishOption.PacketIdentifier & 255);
            Data.Add(msb);
            Data.Add(lsb);
        }
    }


    protected override void PushPayload()
    {
        Data.AddRange(_msgByte);
    }

    public override PublishOption Decode()
    {
        var option = new PublishOption();

        var helper = _buffer.GetReaderHelper();
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
            option.PacketIdentifier = packetIdentifier;

            msgLength -= 2;
        }

        var msg = helper.NextStr(msgLength);


        option.TopicName = topic;
        option.Message = msg;
        option.Retain = retain;
        option.QoS = (byte)qos;
        option.Dup = dup;


        return option;
    }
}