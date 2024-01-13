using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class SubscribePacket : AbstractDataPacket<SubscribeOption>
{
    private readonly byte[] _subjectByte;
    private readonly SubscribeOption _subscribeOption;


    public SubscribePacket(SubscribeOption subscribeOption)
    {
        _subscribeOption = subscribeOption;

        _subjectByte = _subscribeOption.TopicName.ToBytes();
    }

    protected override void PushHeaders()
    {
        //固定报头
        Data.Add(PacketType.SUBSCRIBE << 4);
    }

    protected override void PushRemainingLength()
    {
        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        var len = 2 + 2 + _subjectByte.Length + 1;

        foreach (var l in UtilHelpers.ComputeRemainingLength(len)) Data.Add(Convert.ToByte(l));
    }

    protected override void PushVariableHeader()
    {
        var msb = UtilHelpers.RandomByte();
        var lsb = UtilHelpers.RandomByte();

        Data.Add(msb);
        Data.Add(lsb);
    }

    protected override void PushPayload()
    {
        //有效载荷
        //CONNECT 报文的有效载荷（payload）包含一个或多个以长度为前缀的字段，可变报头中的标志决定是否包含这些字段。如果包含的话，必须按这个顺序出现：客户端标识符，遗嘱主题，遗嘱消息，用户名，密码。
        Data.Add((byte)(_subjectByte.Length >> 8));
        Data.Add((byte)(_subjectByte.Length & 255));
        Data.AddRange(_subjectByte);

        Data.Add(_subscribeOption.QoS);
    }

    public override SubscribeOption Decode()
    {
        throw new NotImplementedException();
    }
}