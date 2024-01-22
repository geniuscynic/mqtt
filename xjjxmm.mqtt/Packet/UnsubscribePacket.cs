namespace xjjxmm.mqtt.Packet;

internal class UnSubscribePacket :MqttPacket
{
    public List<string> TopicFilters { get; set; } = new List<string>();
    
}