using mqtt.server.Constant;
using mqtt.server.Options;

namespace mqtt.server.Packet;

internal class PubCompPacket : AbstractDataPacket
{
    private readonly byte msb;
    private readonly byte lsb;

    public PubCompPacket() {}
    public PubCompPacket(int packetIdentifier)
    {
        msb = (byte)(packetIdentifier >> 8);
        lsb = (byte)(packetIdentifier & 255);
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

    public override IOption Decode(ReceivedPacket buffer)
    {
        throw new NotImplementedException();
    }
}
