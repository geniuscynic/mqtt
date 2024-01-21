using mqtt.server.Options;

namespace xjjxmm.mqtt.Options;

public class UnSubscribeOption : IOption
{
    public List<string> TopicFilters { get; set; } = new List<string>();
}