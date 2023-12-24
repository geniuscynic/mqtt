using mqtt.server.Options;

namespace mqtt.server.Packet;

internal class UnSubscribePacket : AbstractDataPacket
{
    private readonly UnSubscribeOption _option;

    public UnSubscribePacket(UnSubscribeOption option)
    {
        _option = option;
    }
    
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

    public override IOption Decode()
    {
        throw new NotImplementedException();
    }
}