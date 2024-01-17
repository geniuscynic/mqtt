using mqtt.server.Constant;
using mqtt.server.Options;
using xjjxmm.mqtt.Constant;

namespace xjjxmm.mqtt.MqttPacket;

internal class PubCompPacket : AbstractDataPacket<PubCompOption>
{
    private readonly ReceivedPacket _buffer;
    private readonly PubCompOption _option;

    public PubCompPacket(PubCompOption option)
    {
        _option = option;
    }

    public PubCompPacket(ReceivedPacket buffer)
    {
        _buffer = buffer;
    }

    protected override void PushHeaders()
    {
        byte header = PacketType.PUBCOMP << 4;

        Data.Add(header);
    }

    protected override void PushRemainingLength()
    {
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

    public override PubCompOption Decode()
    {
        var helper = _buffer.GetReaderHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        return new PubCompOption
        {
            PacketIdentifier = packetIdentifier
        };
    }
}