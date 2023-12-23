using mqtt.server.Constant;
using mqtt.server.Options;

namespace mqtt.server.Packet;

internal class PubAckPacket : AbstractDataPacket
{
    private readonly byte msb;
    private readonly byte lsb;

    public PubAckPacket()
    {
        
    }

    public PubAckPacket(PubAckOption option)
    {
        
    }
    
    public PubAckPacket(int packetIdentifier)
    {
        msb = (byte)(packetIdentifier >> 8);
        lsb = (byte)(packetIdentifier & 255);
    }

    protected override void PushHeaders()
    {
        byte header = PacketType.PUBACK << 4;

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
        return new PubAckOption();
    }
}
