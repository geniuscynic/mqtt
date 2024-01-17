using mqtt.server.Constant;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PubRecPacket : AbstractDataPacket<PubRecOption>
{
    private readonly ReceivedPacket _buffer;
    private readonly PubRecOption _option;
    private readonly byte lsb;
    private readonly byte msb;

    public PubRecPacket(ReceivedPacket buffer)
    {
        _buffer = buffer;
    }

    public PubRecPacket(PubRecOption option)
    {
        _option = option;
    }


    protected override void PushHeaders()
    {
        byte header = PacketType.PUBREC << 4;

        Data.Add(header);
    }

    protected override void PushRemainingLength()
    {
        Data.Add(0x02);
    }

    protected override void PushVariableHeader()
    {
        Data.Add(msb);
        Data.Add(lsb);
    }

    protected override void PushPayload()
    {
    }

    public override PubRecOption Decode()
    {
        var helper = _buffer.GetReaderHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        return new PubRecOption
        {
            PacketIdentifier = packetIdentifier
        };
    }
}