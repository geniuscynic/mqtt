using mqtt.server.Options;
using XjjXmm.Infrastructure.Common;

namespace mqtt.server.Packet;

internal class SubAckPacket : AbstractDataPacket
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

    public override IOption Decode(ReceivedPacket packet)
    {
        var option = new SubAckOption();
        var readerHelper = packet.GetReaderHelper();
        var ids = readerHelper.NextTwoByteInt();
        while (readerHelper.HasNext())
        {
            option.ReasonCodes.Add(readerHelper.Next());
        }

        return option;
    }
}
