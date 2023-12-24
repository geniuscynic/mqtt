using mqtt.server.Options;

namespace mqtt.server.Packet;

interface IDataPacket
{
    ArraySegment<byte> Encode();

    IOption Decode();
}