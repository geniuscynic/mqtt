using mqtt.server.Options;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Packet;

internal class PingRespPacket : AbstractDataPacket<PingRespOption>
{
    protected override void PushHeaders()
    {
        throw new NotImplementedException();
    }

    protected override void PushRemainingLength()
    {
        throw new NotImplementedException();
    }

    protected override void PushVariableHeader()
    {
        throw new NotImplementedException();
    }

    protected override void PushPayload()
    {
        throw new NotImplementedException();
    }

    public override PingRespOption Decode()
    {
        return new PingRespOption();
    }
}