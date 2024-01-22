namespace xjjxmm.mqtt.Packet;

internal class SubscribePacket : MqttPacket
{
    public string TopicName { get; set; }
    
    public byte QoS { get; set; }
}