using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class SubscribePacket : MqttPacket
{
    public string TopicName { get; set; }
    
    public byte QoS { get; set; }
}