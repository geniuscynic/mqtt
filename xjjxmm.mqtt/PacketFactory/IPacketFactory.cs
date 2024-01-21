using mqtt.server.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal interface IPacketFactory
{
    MqttPacket GetPacket();
 
    ArraySegment<byte> Encode();

    IOption GetOption();
}