using mqtt.server.Options;

namespace mqtt.server.Packet;

internal class PingRespPacket : AbstractDataPacket
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

    public override IOption Decode(ReceivedPacket buffer)
    {
        return new PingRespOption();
    }
}