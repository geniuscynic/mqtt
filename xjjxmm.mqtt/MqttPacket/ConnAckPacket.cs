using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class ConnAckPacket : AbstractDataPacket<ConnAckOption>
{
    private readonly ReceivedPacket _buffer;

    public ConnAckPacket(ReceivedPacket buffer)
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

    public override ConnAckOption Decode()
    {
        ConnAckOption option = new();
        var readerHelper = _buffer.GetReaderHelper();
        option.IsSessionPresent = readerHelper.Next() == 1;
        option.ReasonCode = readerHelper.Next();

        return option;
    }
}