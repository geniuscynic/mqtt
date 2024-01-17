using mqtt.server.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal interface IDataPacket : IPacket
{
    ArraySegment<byte> Encode(IOption option);

    IOption Decode(ReceivedPacket data);
}