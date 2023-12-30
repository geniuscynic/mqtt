using mqtt.server.Constant;
using mqtt.server.Options;

namespace xjjxmm.mqtt.Packet;

internal class PingReqPacket : AbstractDataPacket<PingReqOption>
{
    public PingReqPacket(PingReqOption options)
    {
        
    }
    protected override void PushHeaders()
    {
        byte header = PacketType.PINGREQ << 4;

        Data.Add(header);
    }

    protected override void PushRemainingLength()
    {
        Data.Add(0x00);
    }

    protected override void PushVariableHeader()
    {

    }

    protected override void PushPayload()
    {

    }

    public override PingReqOption Decode()
    {
        throw new NotImplementedException();
    }
}