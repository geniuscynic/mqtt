using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class AdaptFactory
{
    public static IAdaptFactory? CreatePacketFactory(IOption option, ushort packetIdentifier = 0)
    {
        return option switch
        {
            ConnectOption connectOption => new ConnectPacketAdapt(connectOption),
            ConnAckOption connAckOption => new ConnAckPacketAdapt(connAckOption),
            PingReqOption pingReqOption => new PingReqPacketAdapt(pingReqOption),
            PingRespOption pingRespOption => new PingRespPacketAdapt(pingRespOption),
            SubscribeOption subscribeOption => new SubscribePacketAdapt(subscribeOption),
            SubAckOption subAckOption => new SubAckPacketAdapt(subAckOption, packetIdentifier),
            UnSubscribeOption unSubscribeOption => new UnSubscribePacketAdapt(unSubscribeOption),
            UnSubAckOption unSubAckOption => new UnSubAckPacketAdapt(unSubAckOption, packetIdentifier),
            PublishOption publishOption => new PublishPacketFactory(publishOption, packetIdentifier),
            PubAckOption pubAckOption => new PubAckPacketAdapt(pubAckOption, packetIdentifier),
            PubRecOption pubRecOption => new PubRecPacketAdapt(pubRecOption, packetIdentifier),
            PubRelOption pubRelOption => new PubRelPacketAdapt(pubRelOption, packetIdentifier),
            PubCompOption pubCompOption => new PubCompPacketAdapt(pubCompOption, packetIdentifier),
            _ => null
        };
    }
    
    public static IAdaptFactory? CreatePacketFactory(Packet.MqttPacket mqttPacket)
    {
        return mqttPacket switch
        {
            ConnectPacket connectOption => new ConnectPacketAdapt(connectOption),
            ConnAckPacket connAckOption => new ConnAckPacketAdapt(connAckOption),
            PingReqPacket pingReqOption => new PingReqPacketAdapt(pingReqOption),
            PingRespPacket pingRespOption => new PingRespPacketAdapt(pingRespOption),
            SubscribePacket subscribeOption => new SubscribePacketAdapt(subscribeOption),
            SubAckPacket subAckOption => new SubAckPacketAdapt(subAckOption),
            UnSubscribePacket unSubscribeOption => new UnSubscribePacketAdapt(unSubscribeOption),
            UnSubAckPacket unSubAckOption => new UnSubAckPacketAdapt(unSubAckOption),
            PublishPacket publishOption => new PublishPacketFactory(publishOption),
            PubAckPacket pubAckOption => new PubAckPacketAdapt(pubAckOption),
            PubRecPacket pubRecOption => new PubRecPacketAdapt(pubRecOption),
            PubRelPacket pubRelOption => new PubRelPacketAdapt(pubRelOption),
            PubCompPacket pubCompOption => new PubCompPacketAdapt(pubCompOption),
            _ => null
        };
    }
    
    public static IAdaptFactory? CreatePacketFactory(ReceivedPacket receivedPacket)
    {
        if (receivedPacket.GetPacketType() == PacketType.Connect)
        {
            return new ConnectPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.ConnAck)
        {
            return new ConnAckPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PingReq)
        {
            return new PingReqPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PingResp)
        {
            return new PingRespPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.Subscribe)
        {
            return new SubscribePacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.SubAck)
        {
            return new SubAckPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.UnSubscribe)
        {
            return new UnSubscribePacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.UnSubAck)
        {
            return new UnSubAckPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.Publish)
        {
            return new PublishPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubAck)
        {
            return new PubAckPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubRec)
        {
            return new PubRecPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubRel)
        {
            return new PubRelPacketAdapt(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubComp)
        {
            return new PubCompPacketAdapt(receivedPacket);
        }

        return null;
    }
}