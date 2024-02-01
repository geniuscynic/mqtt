namespace xjjxmm.mqtt.Packet;

internal class UnSubscribePacket :IdentifierPacket
{
    public List<string> TopicNames { get; set; } = new List<string>();
    
}