using mqtt.server.Options;
using xjjxmm.mqtt.Options;

namespace mqtt.server.Packet;

internal class ConnAckPacket : AbstractDataPacket
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
        ConnAckOption option = new();
        var readerHelper = buffer.GetReaderHelper();
        option.IsSessionPresent = readerHelper.Next() ==  1;
        option.ReasonCode = readerHelper.Next();

        return option;
    }
}
