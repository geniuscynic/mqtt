namespace xjjxmm.mqtt.Packet;

internal class SubscribePacket : IdentifierPacket
{
    public string TopicName { get; set; }
    
    public byte QoS { get; set; }
}