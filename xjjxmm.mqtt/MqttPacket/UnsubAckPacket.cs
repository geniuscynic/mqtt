using mqtt.server.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class UnSubAckPacket : MqttPacket
{
    public int PacketIdentifier { get; set; }
    
}