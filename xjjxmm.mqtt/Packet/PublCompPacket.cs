using mqtt.server;
using mqtt.server.Constant;
using mqtt.server.Options;

namespace xjjxmm.mqtt.Packet;

internal class PubCompPacket : AbstractDataPacket<PubCompOption>
{
    private readonly byte msb;
    private readonly byte lsb;
    private readonly ReceivedPacket _buffer;

    public PubCompPacket(int packetIdentifier)
    {
        msb = (byte)(packetIdentifier >> 8);
        lsb = (byte)(packetIdentifier & 255);
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

        return new PubCompOption()
        {
            PacketIdentifier = packetIdentifier
        };
    }
}
