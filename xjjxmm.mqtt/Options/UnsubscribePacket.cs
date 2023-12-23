using mqtt.server.Options;

namespace mqtt.server.Packet;

public class UnSubscribeOption : IOption
{
    public List<string> TopicFilters { get; set; } = new List<string>();
}