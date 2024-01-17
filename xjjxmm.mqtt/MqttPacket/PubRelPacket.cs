using mqtt.server.Constant;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PubRelPacket : AbstractDataPacket<PubRelOption>
{
    private readonly ReceivedPacket _buffer;
    private readonly PubRelOption _option;

    public PubRelPacket(ReceivedPacket buffer)
    {
        _buffer = buffer;
    }

    /*public PubRelPacket(int packetIdentifier)
    {

    }*/

    public PubRelPacket(PubRelOption option)
    {
        _option = option;
    }

    protected override void PushHeaders()
    {
        var header = (PacketType.PUBREL << 4) | 0x02;

        Data.Add(Convert.ToByte(header));
    }

    protected override void PushRemainingLength()
    {
        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        Data.Add(0x02);
    }

    protected override void PushVariableHeader()
    {
        var msb = (byte)(_option.PacketIdentifier >> 8);
        var lsb = (byte)(_option.PacketIdentifier & 255);

        Data.Add(msb);
        Data.Add(lsb);
    }


    protected override void PushPayload()
    {
    }

    public override PubRelOption Decode()
    {
        var helper = _buffer.GetReaderHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        return new PubRelOption
        {
            PacketIdentifier = packetIdentifier
        };
    }
}