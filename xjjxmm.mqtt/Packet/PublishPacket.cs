using mqtt.client.test;
using mqtt.server;
using mqtt.server.Constant;
using mqtt.server.Options;
using mqtt.server.Packet;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Packet;

internal class PublishPacket(PublishOption publishOption) : AbstractDataPacket<PublishOption>
{
    private readonly byte[] _subjectByte = publishOption.TopicName.ToBytes();
    private readonly byte[] _msgByte = publishOption.Message.ToBytes();
    
    protected override void PushHeaders()
    {
        var header = PacketType.PUBLISH << 4;

        if (publishOption.Dup)
        {
            header |= 0x01 << 3;
        }

        header |= publishOption.QoS << 1;

        if (publishOption.Retain)
        {
            header |= 0x01;
        }

        Data.Add(Convert.ToByte(header));
    }

    protected override void PushRemainingLength()
    {

        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        var len = 2 + _subjectByte.Length + _msgByte.Length;
        if (publishOption.QoS > 0)
        {
            len += 2;
        }

        foreach (var l in UtilHelpers.ComputeRemainingLength(len))
        {
            Data.Add(Convert.ToByte(l));
        }
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
        throw new NotImplementedException();
    }
}