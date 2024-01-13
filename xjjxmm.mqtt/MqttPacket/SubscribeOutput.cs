using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

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
        var packetIdentifier = readerHelper.NextTwoByteInt();
        option.PacketIdentifier = packetIdentifier;
        while (readerHelper.HasNext()) option.ReasonCodes.Add(readerHelper.Next());

        return option;
    }
}