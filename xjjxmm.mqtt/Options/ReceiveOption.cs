namespace xjjxmm.mqtt.Options;

public record ReceiveOption(string TopicName, string Message) : IOption
{

}