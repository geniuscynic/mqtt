using mqtt.server.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal interface IDataPacket<T> where T : IOption
{
    ArraySegment<byte> Encode();

    T Decode();
}