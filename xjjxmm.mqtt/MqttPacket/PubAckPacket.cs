using mqtt.server.Constant;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PubAckPacket : MqttPacket
{
    public ushort PacketIdentifier { get; set; }
  
}