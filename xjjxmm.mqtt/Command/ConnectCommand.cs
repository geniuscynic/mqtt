﻿using mqtt.server.Constant;
using mqtt.server.Options;
using xjjxmm.mqtt.Channel;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Command;

internal class ConnectCommand(ConnectOption option) : BaseCommand
{
    public override byte? AcceptCommand => PacketType.CONNACK;
    public override ArraySegment<byte> Encode()
    {
        return new ConnectPacket(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
        return new ConnAckPacket(data).Decode();
    }
}

internal class PingReqCommand(PingReqOption option) : BaseCommand
{
    public override byte? AcceptCommand => PacketType.PINGRESP;
    public override ArraySegment<byte> Encode()
    {
        return new PingReqPacket(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
        return new PingRespPacket(data).Decode();
    }
}

internal class PublishAtMostOnceCommand(PublishOption option) : BaseCommand
{
    public override byte? AcceptCommand => null;
    public override ArraySegment<byte> Encode()
    {
        return new PublishPacket(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
        return null;
    }
}

internal class PublishAtLeastOnceCommand(PublishOption option) : BaseCommand
{
    public override byte? AcceptCommand =>  PacketType.PUBACK;
    public override ArraySegment<byte> Encode()
    {
        return new PublishPacket(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
        return new PubAckPacket(data).Decode();
    }
}

internal class PublishExactlyOnceCommand(PublishOption option) : BaseCommand
{
    public override byte? AcceptCommand =>  PacketType.PUBREC;
    public override ArraySegment<byte> Encode()
    {
        return new PublishPacket(option).Encode();
    }
    
    public override IOption Decode(ReceivedPacket data)
    {
        return new PubRecPacket(data).Decode();
    }
}

internal class PubRelCommand(PubRelOption option) : BaseCommand
{
    public override byte? AcceptCommand =>  PacketType.PUBCOMP;
    public override ArraySegment<byte> Encode()
    {
        return new PubRelPacket(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
        return new PubCompPacket(data).Decode();
    }
}

internal class SubscribeCommand(SubscribeOption option) : BaseCommand
{
    public override byte? AcceptCommand =>  PacketType.SUBACK;
    public override ArraySegment<byte> Encode()
    {
        return new SubscribePacket(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
      return  new SubAckPacket(data).Decode();
    }
}

internal class ReceivePublishCommand : ICommand
{
    public IOption Decode(ReceivedPacket data)
    {
        return new PublishPacket(data).Decode();
    }

    public TaskCompletionSource<ReceivedPacket> Result { get; } = new();
    public byte? AcceptCommand { get; } = PacketType.PUBLISH;
}

internal class PubAckCommand(PubAckOption option) : BaseCommand
{
    public override byte? AcceptCommand => null;

    public override ArraySegment<byte> Encode()
    {
        return new PubAckPacket(option).Encode();
    }
    
    public override IOption Decode(ReceivedPacket data)
    {
        return null;
    }
}

internal class PubRecCommand(PubRecOption option) : BaseCommand {
    public override byte? AcceptCommand => PacketType.PUBREL;
    public override ArraySegment<byte> Encode()
    {
        return new PubRecPacket(option).Encode();
    }
    
    public override IOption Decode(ReceivedPacket data)
    {
        return new PubRelPacket(data).Decode();
    }
}

internal class PubCompCommand(PubCompOption option) : BaseCommand
{
    public override byte? AcceptCommand => null;
    public override ArraySegment<byte> Encode()
    {
        return new PubCompPacket(option).Encode();
    }
    public override IOption Decode(ReceivedPacket data)
    {
        return null;
    }
}