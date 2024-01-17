using System.Runtime.CompilerServices;
using mqtt.server.Constant;
using mqtt.server.Options;
using mqtt.server.Packet;
using xjjxmm.mqtt.Channel;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Command;

internal class CommandFactory
{
    public ICommand Create(MqttChannel3 mqttChannel, IOption option)
    {
            if (option is ConnectOption connectOption)
            {
                var packet = new ConnectPacket().Decode(connectOption);
                var command = new ConnectCommand(mqttChannel,packet);
                
                return command;
            }

            return null;
    }
}

internal enum CommandEnum
{
    SendConnect,
    ReceiveConnect,
    SendPublish,
    ReceivePublish,
    
}
internal class Command
{
    public CommandEnum Name { get; set; }
    public IOption? Option { get; set; }
    public ReceivedPacket? Packet { get; set; }

    private static Command Create(CommandEnum name, IOption? option, ReceivedPacket? packet)
    {
        return new Command()
        {
            Name = name,
            Option = option,
            Packet = packet
        };
    }
    
    public static Command Create(CommandEnum name, IOption option)
    {
        return Create(name, option, null);
    }
    
    public static Command Create(CommandEnum name, ReceivedPacket packet)
    {
        return Create(name, null, packet);
    }
}

internal class ConnectCommand(MqttChannel3 mqttChannel,IPacket packet) : ICommand
{
    public async Task Send()
    {
        await mqttChannel.Connect((ConnectPacket)packet);
        await mqttChannel.Send(packet.Encode());
    }

    public Task<IOption> GetResult()
    {
        throw new NotImplementedException();
    }
}

internal class PublisCommand(MqttChannel3 mqttChannel) : ICommand
{
    public async Task Send(IOption option)
    {
        var packet = new PublishPacket1().Encode(option);
        await mqttChannel.Send(packet);
    }
    
    public Task<IOption> GetResult()
    {
        throw new NotImplementedException();
    }

    public Task SetResult(ReceivedPacket receivedPacket)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsHandel(ReceivedPacket receivedPacket)
    {
        throw new NotImplementedException();
    }
}

internal class PingReqCommand(PingReqOption option) : BaseCommand
{
    public override PacketType AcceptCommand => PacketType.PingResp;
    public override ArraySegment<byte> Encode()
    {
        return new PingReqPacket(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
        return new PingRespPacket(data).Decode();
    }
}



internal class PublishAtMostOnceCommand(PublishOption option) : ICommand
{
    public ArraySegment<byte> Encode()
    {
        return new PublishPacket1(option).Encode();
    }
}

internal class PublishAtLeastOnceCommand(PublishOption option) : BaseCommand
{
    public override PacketType AcceptCommand =>  PacketType.PUBACK;
    public override ArraySegment<byte> Encode()
    {
        return new PublishPacket1(option).Encode();
    }

    public override IOption Decode(ReceivedPacket data)
    {
        return new PubAckPacket(data).Decode();
    }
}

internal class PublishExactlyOnceCommand(PublishOption option) : BaseCommand
{
    public override PacketType AcceptCommand =>  PacketType.PUBREC;
    public override ArraySegment<byte> Encode()
    {
        return new PublishPacket1(option).Encode();
    }
    
    public override IOption Decode(ReceivedPacket data)
    {
        return new PubRecPacket(data).Decode();
    }
}

internal class PubRelCommand(PubRelOption option) : BaseCommand
{
    public override PacketType AcceptCommand =>  PacketType.PUBCOMP;
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
    public override PacketType AcceptCommand =>  PacketType.SUBACK;
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
        return new PublishPacket1(data).Decode();
    }

    public TaskCompletionSource<ReceivedPacket> Result { get; } = new();
    public PacketType AcceptCommand { get; } = PacketType.PUBLISH;
}

internal class PubAckCommand(PubAckOption option) : BaseCommand
{
    public override PacketType AcceptCommand => null;

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
    public override PacketType AcceptCommand => PacketType.PUBREL;
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
    public override PacketType AcceptCommand => null;
    public override ArraySegment<byte> Encode()
    {
        return new PubCompPacket(option).Encode();
    }
    public override IOption Decode(ReceivedPacket data)
    {
        return null;
    }
}