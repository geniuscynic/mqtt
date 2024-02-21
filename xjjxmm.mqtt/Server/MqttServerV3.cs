using System.Net;
using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Adapt;
using xjjxmm.mqtt.Client;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Server;

public class MqttServerV3 
{
    private async Task OnConnect(SocketInfo socketInfo, ConnectPacket connPacket)
    {
        var connAckPacket = new ConnAckPacket()
        {
            ReasonCode = ConnectReturnCode.Accepted
        };
        if (connPacket.ProtocolLevel == MqttProtocolLevel.V311
            || connPacket.ProtocolLevel == MqttProtocolLevel.V5)
        {
         
        }
        else
        {
            connAckPacket.ReasonCode = ConnectReturnCode.UnsupportedProtocolVersion;
            var packetFactory = AdaptFactory.CreatePacketFactory(connAckPacket);
            await socketInfo.Channel.Send(packetFactory!);
            socketInfo.Channel.Dispose();
        }
    }
}
