using mqtt.server;
using mqtt.server.Options;
using mqtt.server.Packet;

namespace xjjxmm.mqtt.Packet;

internal class SubAckPacket : AbstractDataPacket<SubAckOption>
{
    private readonly ReceivedPacket _buffer;

    public SubAckPacket(ReceivedPacket buffer)
    {
        _buffer = buffer;
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

    public override SubAckOption Decode()
    {
        var option = new SubAckOption();
        var readerHelper = _buffer.GetReaderHelper();
        var ids = readerHelper.NextTwoByteInt();
        while (readerHelper.HasNext())
        {
            option.ReasonCodes.Add(readerHelper.Next());
        }

        return option;
    }
}
