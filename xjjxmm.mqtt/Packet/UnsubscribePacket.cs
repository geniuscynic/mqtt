using mqtt.server.Options;
using mqtt.server.Packet;

namespace xjjxmm.mqtt.Packet;

internal class UnSubscribePacket : AbstractDataPacket<UnSubscribeOption>
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

    public override UnSubscribeOption Decode()
    {
        throw new NotImplementedException();
    }
}