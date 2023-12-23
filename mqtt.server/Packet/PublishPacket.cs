using mqtt.client.test;
using mqtt.server.Constant;
using mqtt.server.Options;
using Xjjxmm.Infrastructure.ToolKit;

namespace mqtt.server.Packet;

internal class PublishPacket : AbstractDataPacket
{
    private readonly PublishOption _publishOption;
    private readonly byte[] _subjectByte;
    private readonly byte[] _msgByte;
    private readonly byte msb;
    private readonly byte lsb;

    public PublishPacket(PublishOption publishOption)
    {
        _publishOption = publishOption;

        _subjectByte = publishOption.TopicName.ToBytes();

        _msgByte = publishOption.Message.ToBytes();

        msb = RandomKit.RandomByte();
        lsb = RandomKit.RandomByte();

        PacketIdentifier = msb << 8 | lsb;
    }

    public int PacketIdentifier
    {
        get;
    }

    public PublishOption option => _publishOption;

    protected override void PushHeaders()
    {
        var header = PacketType.PUBLISH << 4;

        if (_publishOption.Dup)
        {
            header |= 0x01 << 3;
        }

        header |= _publishOption.QoS << 1;

        if (_publishOption.Retain)
        {
            header |= 0x01;
        }

        Data.Add(Convert.ToByte(header));
    }

    protected override void PushRemainingLength()
    {

        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        var len = 2 + _subjectByte.Length + _msgByte.Length;
        if (_publishOption.QoS > 0)
        {
            len += 2;
        }

        foreach (var l in client.test.Util.ComputeRemainingLength(len))
        {
            Data.Add(Convert.ToByte(l));
        }
    }

    protected override void PushVariableHeader()
    {
        Data.Add((byte)(_subjectByte.Length >> 8));
        Data.Add((byte)(_subjectByte.Length & 255));
        Data.AddRange(_subjectByte);

        //todo
        // 报文标识符 Packet Identifier
        if (_publishOption.QoS > 0)
        {
            Data.Add(msb);
            Data.Add(lsb);
        }
    }


    protected override void PushPayload()
    {
        //有效载荷
        //CONNECT 报文的有效载荷（payload）包含一个或多个以长度为前缀的字段，可变报头中的标志决定是否包含这些字段。如果包含的话，必须按这个顺序出现：客户端标识符，遗嘱主题，遗嘱消息，用户名，密码。

        Data.AddRange(_msgByte);


    }

    public override IOption Decode(ReceivedPacket buffer)
    {
        throw new NotImplementedException();
    }
}