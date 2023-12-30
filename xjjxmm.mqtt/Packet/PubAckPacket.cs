using mqtt.server;
using mqtt.server.Constant;
using mqtt.server.Options;
using mqtt.server.Packet;
using mqtt.server.Util;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Packet;

internal class PubAckPacket : AbstractDataPacket<PubAckOption>
{
    private readonly PubAckOption _option;
    private readonly ReceivedPacket _buffer;
    private readonly byte msb;
    private readonly byte lsb;

    public PubAckPacket(ReceivedPacket buffer)
    {
        _buffer = buffer;
    }

    public PubAckPacket(PubAckOption option)
    {
        _option = option;
    }
    
    /*public PubAckPacket(int packetIdentifier)
    {
        msb = (byte)(packetIdentifier >> 8);
        lsb = (byte)(packetIdentifier & 255);
    }*/

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

    public override PubAckOption Decode()
    {
        var helper = _buffer.GetReaderHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        return new PubAckOption
        {
            PacketIdentifier = packetIdentifier
        };
    }
}
