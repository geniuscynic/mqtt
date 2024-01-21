using mqtt.server.Options;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.PacketFactory;

internal class PacketFactory
{
    public static IPacketFactory? CreatePacketFactory(IOption option)
    {
        return option switch
        {
            ConnectOption connectOption => new ConnectPacketFactory(connectOption),
            ConnAckOption connAckOption => new ConnAckPacketFactory(connAckOption),
            PingReqOption pingReqOption => new PingReqPacketFactory(pingReqOption),
            PingRespOption pingRespOption => new PingRespPacketFactory(pingRespOption),
            SubscribeOption subscribeOption => new SubscribePacketFactory(subscribeOption),
            SubAckOption subAckOption => new SubAckPacketFactory(subAckOption),
            UnSubscribeOption unSubscribeOption => new UnSubscribePacketFactory(unSubscribeOption),
            UnSubAckOption unSubAckOption => new UnSubAckPacketFactory(unSubAckOption),
            PublishOption publishOption => new PublishPacketFactory(publishOption),
            PubAckOption pubAckOption => new PubAckPacketFactory(pubAckOption),
            PubRecOption pubRecOption => new PubRecPacketFactory(pubRecOption),
            PubRelOption pubRelOption => new PubRelPacketFactory(pubRelOption),
            PubCompOption pubCompOption => new PubCompPacketFactory(pubCompOption),
            _ => null
        };
    }
    
    public static IPacketFactory? CreatePacketFactory(MqttPacket.MqttPacket mqttPacket)
    {
        return mqttPacket switch
        {
            ConnectPacket connectOption => new ConnectPacketFactory(connectOption),
            ConnAckPacket connAckOption => new ConnAckPacketFactory(connAckOption),
            PingReqPacket pingReqOption => new PingReqPacketFactory(pingReqOption),
            PingRespPacket pingRespOption => new PingRespPacketFactory(pingRespOption),
            SubscribePacket subscribeOption => new SubscribePacketFactory(subscribeOption),
            SubAckPacket subAckOption => new SubAckPacketFactory(subAckOption),
            UnSubscribePacket unSubscribeOption => new UnSubscribePacketFactory(unSubscribeOption),
            UnSubAckPacket unSubAckOption => new UnSubAckPacketFactory(unSubAckOption),
            PublishPacket publishOption => new PublishPacketFactory(publishOption),
            PubAckPacket pubAckOption => new PubAckPacketFactory(pubAckOption),
            PubRecPacket pubRecOption => new PubRecPacketFactory(pubRecOption),
            PubRelPacket pubRelOption => new PubRelPacketFactory(pubRelOption),
            PubCompPacket pubCompOption => new PubCompPacketFactory(pubCompOption),
            _ => null
        };
    }
    
    public static IPacketFactory? CreatePacketFactory(ReceivedPacket receivedPacket)
    {
        if (receivedPacket.GetPacketType() == PacketType.Connect)
        {
            return new ConnectPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.ConnAck)
        {
            return new ConnAckPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PingReq)
        {
            return new PingReqPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PingResp)
        {
            return new PingRespPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.Subscribe)
        {
            return new SubscribePacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.SubAck)
        {
            return new SubAckPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.UnSubscribe)
        {
            return new UnSubscribePacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.UnSubAck)
        {
            return new UnSubAckPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.Publish)
        {
            return new PublishPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubAck)
        {
            return new PubAckPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubRec)
        {
            return new PubRecPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubRel)
        {
            return new PubRelPacketFactory(receivedPacket);
        }
        else if (receivedPacket.GetPacketType() == PacketType.PubComp)
        {
            return new PubCompPacketFactory(receivedPacket);
        }

        return null;
    }
    
    
}