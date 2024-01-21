using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class UnSubscribePacket :MqttPacket
{
    public List<string> TopicFilters { get; set; } = new List<string>();
    
}