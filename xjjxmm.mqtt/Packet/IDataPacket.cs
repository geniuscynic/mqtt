using mqtt.server.Options;

namespace xjjxmm.mqtt.Packet;

interface IDataPacket<T> where T:IOption
{
    ArraySegment<byte> Encode();

    T Decode();
}