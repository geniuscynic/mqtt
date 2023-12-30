using mqtt.server;
using mqtt.server.Constant;
using mqtt.server.Options;
using mqtt.server.Packet;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Packet;

internal class PubRecPacket : AbstractDataPacket<PubRecOption>
{
    private readonly PubRecOption _option;
    private readonly ReceivedPacket _buffer;
    private readonly byte msb;
    private readonly byte lsb;

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

        return new PubRecOption()
        {
            PacketIdentifier = packetIdentifier
        };
    }
}
