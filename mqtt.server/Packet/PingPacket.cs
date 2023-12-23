using System.Net.NetworkInformation;
using mqtt.server.Constant;
using mqtt.server.Options;

namespace mqtt.server.Packet;

internal class PingReqPacket : AbstractDataPacket
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

    public override IOption Decode(ReceivedPacket buffer)
    {
        throw new NotImplementedException();
    }
}