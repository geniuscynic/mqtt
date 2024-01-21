using mqtt.server.Constant;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PubRecPacket : MqttPacket
{
    public int PacketIdentifier { get; set; }
}