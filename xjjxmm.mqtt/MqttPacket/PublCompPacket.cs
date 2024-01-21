using mqtt.server.Constant;
using mqtt.server.Options;
using xjjxmm.mqtt.Constant;

namespace xjjxmm.mqtt.MqttPacket;

internal class PubCompPacket : MqttPacket
{
    public int PacketIdentifier { get; set; }
}