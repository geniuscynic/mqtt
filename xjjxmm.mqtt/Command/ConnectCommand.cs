using mqtt.server.Constant;
using mqtt.server.Util;
using xjjxmm.mqtt.Channel;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Command;

internal class ConnectCommand(ConnectOption option) : BaseCommand
{
    public override byte AcceptCommand => PacketType.CONNACK;
    public override ArraySegment<byte> Encode()
    {
        return new ConnectPacket(option).Encode();
    }
}