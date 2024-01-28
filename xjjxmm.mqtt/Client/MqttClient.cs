using mqtt.server.Constant;
using xjjxmm.mqtt.Adapt;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Client;

public class MqttClient : IDisposable
{
    private readonly MqttChannel _mqttChannel;
    private readonly Dispatcher _dispatcher;
    public Action<ReceiveOption>? ReceiveMessage { get; set; }
   // private readonly HashSet<int> _packetIdentifierHashSet = new();
    private readonly PacketIdentifierProvider _packetIdentifierProvider = new();

    public MqttClient()
    {
        SocketProxy socketClient = new SocketProxy();
        _mqttChannel = new MqttChannel(socketClient, Receive);
        _dispatcher = new Dispatcher(_mqttChannel);
    }

    public async Task<ConnAckOption> Connect(ConnectOption option)
    {
        //_mqttChannel.Received = Receive;
        
        //var command = Command.Command.Create(CommandEnum.SendConnect, option);
        var packetFactory = AdaptFactory.CreatePacketFactory(option);
        await _mqttChannel.Connect((ConnectPacket)packetFactory!.GetPacket());
        //await _mqttChannel.Send(packetFactory.Encode());
        packetFactory = await _dispatcher.AddEventHandel(packetFactory, PacketType.ConnAck);

        var connAck = (ConnAckOption)packetFactory!.GetOption();

        if (connAck.ReasonCode != ConnectReturnCode.Accepted)
        {
            _mqttChannel.Dispose();
        }

        //Ping(option.KeepAliveSecond * 1000);

        return connAck;
    }


    private async Task Receive(ReceivedPacket receivedPacket)
    {
        var packetType = receivedPacket.GetPacketType();
        if (packetType == PacketType.Publish)
        {
            var packetFactory = AdaptFactory.CreatePacketFactory(receivedPacket);
            var publishPacket = (PublishPacket)packetFactory!.GetPacket();

            if (publishPacket.QoS == Qos.AtMostOnce)
            {
                ReceiveMessage?.Invoke(new ReceiveOption(publishPacket.TopicName, publishPacket.Message));
            }
            else if (publishPacket.QoS == Qos.AtLeastOnce)
            {
                var pubAckPacket = new PubAckPacket()
                {
                    PacketIdentifier = publishPacket.PacketIdentifier
                };

                packetFactory = AdaptFactory.CreatePacketFactory(pubAckPacket);
                await _mqttChannel.Send(packetFactory!.Encode());
              
                ReceiveMessage?.Invoke(new ReceiveOption(publishPacket.TopicName, publishPacket.Message));
            }
            else if (publishPacket.QoS == Qos.ExactlyOnce)
            {
                var pubRecPacket = new PubRecPacket()
                {
                    PacketIdentifier = publishPacket.PacketIdentifier
                };

                packetFactory = AdaptFactory.CreatePacketFactory(pubRecPacket);
                var pubRelFactory = await _dispatcher.AddEventHandel(packetFactory!, PacketType.PubRel);
                var pubRelPacket = (PubRelPacket)pubRelFactory!.GetPacket();
                var compPacket = new PubRecPacket()
                {
                    PacketIdentifier = pubRelPacket.PacketIdentifier
                };
                packetFactory = AdaptFactory.CreatePacketFactory(compPacket);
                await _mqttChannel.Send(packetFactory!.Encode());
                
                ReceiveMessage?.Invoke(new ReceiveOption(publishPacket.TopicName, publishPacket.Message));
            }
        }
        else
        {
            _dispatcher.Dispatch(receivedPacket);
        }
    }
    
    private void Ping(int second)
    {
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                var packetFactory = AdaptFactory.CreatePacketFactory(new PingReqOption());
                //await _mqttChannel.Send(packetFactory.Encode());
                await _dispatcher.AddEventHandel(packetFactory!, PacketType.PingResp);

                await Task.Delay(second);
            }
        }, TaskCreationOptions.LongRunning);
    }

    public async Task Publish(PublishOption option)
    {
        switch (option.QoS)
        {
            case Qos.AtMostOnce:
                await PublishAtMostOnce(option);
                break;
            case Qos.AtLeastOnce:
                await PublishAtLeastOnce(option);
                break;
            case Qos.ExactlyOnce:
                await PublishExactlyOnce(option);
                break;
        }
    }

    private async Task PublishAtMostOnce(PublishOption option)
    {
        var packetFactory = AdaptFactory.CreatePacketFactory(option);
        await _mqttChannel.Send(packetFactory!.Encode());
    }

    private async Task PublishAtLeastOnce(PublishOption option)
    {
        _packetIdentifierProvider.Next();
        /*while (_packetIdentifierHashSet.Contains(_packetIdentifierProvider.Current))
        {
            _packetIdentifierProvider.Next();
        }*/

        //_packetIdentifierHashSet.Add(_packetIdentifierProvider.Current);
        var packetIdentifier = _packetIdentifierProvider.Current;

        var packetFactory = AdaptFactory.CreatePacketFactory(option, packetIdentifier);
        while (true)
        {
            try
            {
                //await _mqttChannel.Send(packetFactory!.Encode());
                await _dispatcher.AddEventHandel(packetFactory!, PacketType.PubAck);
                //var  pubAckPacket = (PubAckPacket) pubAckFactory!.GetPacket();
                break;
                //_packetIdentifierHashSet.Remove(pubAckPacket.PacketIdentifier);
            }
            catch
            {
                var publishPack = (PublishPacket)packetFactory!.GetPacket();
                publishPack.Dup = true;
            }
        }
    }

    private async Task PublishExactlyOnce(PublishOption option)
    {
        _packetIdentifierProvider.Next();
        /*while (_packetIdentifierHashSet.Contains(_packetIdentifierProvider.Current))
        {
            _packetIdentifierProvider.Next();
        }*/

        // _packetIdentifierHashSet.Add(_packetIdentifierProvider.Current);
        var packetIdentifier = _packetIdentifierProvider.Current;

        var packetFactory = AdaptFactory.CreatePacketFactory(option, packetIdentifier);
        while (true)
        {
            try
            {
                //await _mqttChannel.Send(packetFactory!.Encode());
                await _dispatcher.AddEventHandel(packetFactory!, PacketType.PubRec);
                break;
            }
            catch(Exception ex)
            {
                var publishPack = (PublishPacket)packetFactory!.GetPacket();
                publishPack.Dup = true;
            }
        }

        var pubRelPacket = new PubRelPacket()
        {
            PacketIdentifier = packetIdentifier
        };
        packetFactory = AdaptFactory.CreatePacketFactory(pubRelPacket);
        //await _mqttChannel.Send(packetRelFactory!.Encode());
        await _dispatcher.AddEventHandel(packetFactory!, PacketType.PubComp);
    }

    public async Task<SubAckOption> Subscribe(SubscribeOption option)
    {
        _packetIdentifierProvider.Next();
        var packetIdentifier = _packetIdentifierProvider.Current;
        var packetFactory = AdaptFactory.CreatePacketFactory(option, packetIdentifier);

        //await _mqttChannel.Send(packetFactory!.Encode());
        packetFactory = await _dispatcher.AddEventHandel(packetFactory!, PacketType.SubAck);

        var subAckOption = (SubAckOption)packetFactory!.GetOption();

        return subAckOption;
    }

    public void Dispose()
    {
        _mqttChannel.Dispose();
    }
}