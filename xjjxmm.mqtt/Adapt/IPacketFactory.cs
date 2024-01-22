using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Adapt;

internal interface IAdaptFactory
{
    Packet.MqttPacket GetPacket();
 
    ArraySegment<byte> Encode();

    IOption GetOption();
}