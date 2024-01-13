using mqtt.server.Constant;
using mqtt.server.Options;

namespace xjjxmm.mqtt.Net;

internal interface IPacket
{
    PacketTypeEnum GetPacketType();
}