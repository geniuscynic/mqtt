using mqtt.server.Constant;
using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.MqttPacket;

internal record struct ReceivedPacket
{
    private readonly BufferReaderHelper _readerHelper;

    public int TotalLength = 0;

    public ReceivedPacket(byte header, byte remainingLength, ArraySegment<byte> body)
    {
        Header = header;
        _readerHelper = new BufferReaderHelper(body);
        RemainingLength = remainingLength;
        ComputeRemainingLength();
    }

    public byte Header { get; }
    public int RemainingLength { get; private set; }

    public byte GetCommandName()
    {
        switch (Header & 0xF0)
        {
            case PacketType.CONNACK << 4:
                return PacketType.CONNACK;
            case PacketType.PUBLISH << 4:
                return PacketType.PUBLISH;
            case PacketType.PUBACK << 4:
                return PacketType.PUBACK;
            case PacketType.PUBREC << 4:
                return PacketType.PUBREC;
            case PacketType.PUBREL << 4:
                return PacketType.PUBREL;
            case PacketType.PUBCOMP << 4:
                return PacketType.PUBCOMP;
            case PacketType.SUBACK << 4:
                return PacketType.SUBACK;
            case PacketType.PINGRESP << 4:
                return PacketType.PINGRESP;
            case PacketType.UNSUBACK << 4:
                return PacketType.UNSUBACK;
        }

        return 0;
    }

    public void Append(ArraySegment<byte> content)
    {
        _readerHelper.Append(content);
    }

    public BufferReaderHelper GetReaderHelper()
    {
        return _readerHelper;
    }

    /// <summary>
    ///     获取一个长度数据
    /// </summary>
    /// <returns></returns>
    private void ComputeRemainingLength()
    {
        var i = 0;
        var value = RemainingLength & 127;
        var multiplier = 128;

        var encodedByte = RemainingLength;
        while ((encodedByte & 128) != 0)
        {
            var next = _readerHelper.Next();
            value += (next & 127) * multiplier;
            //value++;

            if (multiplier > 128 * 128 * 128)
                throw new Exception("字段太长，超出限制了");

            multiplier *= 128;

            encodedByte = next;

            i++;
        }

        // _readerHelper.Prev();
        RemainingLength = value;

        TotalLength = RemainingLength + i;
    }
}