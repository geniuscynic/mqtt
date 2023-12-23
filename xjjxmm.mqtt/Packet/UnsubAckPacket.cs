using mqtt.server.Options;

namespace mqtt.server.Packet;

internal class UnSubAckPacket : AbstractDataPacket
{
    private readonly UnSubAckOption _option;

    public UnSubAckPacket(UnSubAckOption option)
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

    public override IOption Decode(ReceivedPacket buffer)
    {
        throw new NotImplementedException();
    }
}