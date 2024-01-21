using System.Collections.Concurrent;
using mqtt.server.Constant;
using mqtt.server.Options;
using xjjxmm.mqtt.Channel;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Client;

public class MqttClient : IDisposable
{
    private readonly MqttChannel _mqttChannel = new();
    public Action<string>? ReceiveMessage{ get; set; }
    private readonly HashSet<int> _packetIdentifierHashSet = new();
    private readonly PacketIdentifierProvider _packetIdentifierProvider = new ();

  
    public async Task<ConnAckOption> Connect(ConnectOption option)
    {
        _mqttChannel.Received = Receive;
       

        //var command = Command.Command.Create(CommandEnum.SendConnect, option);
        var packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(option);
        //var packet = packetHelper.CreatePacket(option);
        await _mqttChannel.Connect((ConnectPacket)packetFactory.GetPacket());
        await _mqttChannel.Send(packetFactory.Encode());
        packetFactory = await Dispatcher.Instance.AddEventHandel(PacketType.ConnAck);
        
        var connAck = (ConnAckOption)packetFactory.GetOption();
        
        if (connAck.ReasonCode != ConnectReturnCode.Accepted)
        {
            _mqttChannel.Dispose();
        }
        
       Ping(option.KeepAliveSecond * 1000);
       
        return connAck;
    }
    
    
    private async Task Receive(ReceivedPacket receivedPacket)
    {
        var packetType = receivedPacket.GetPacketType();
        if (packetType == PacketType.Publish)
        {
            var packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(receivedPacket);
            var publishPacket = (PublishPacket)packetFactory!.GetPacket();
            if (publishPacket.QoS == Qos.AtMostOnce)
            {
                var pubAckPacket = new PubAckPacket()
                {
                    PacketIdentifier = publishPacket.PacketIdentifier
                };
                
                packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(pubAckPacket);
                await _mqttChannel.Send(packetFactory!.Encode());
                
            }
            else if (publishPacket.QoS == Qos.ExactlyOnce)
            {
                var pubRecPacket = new PubRecPacket()
                {
                    PacketIdentifier = publishPacket.PacketIdentifier
                };
                
                packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(pubRecPacket);
                await _mqttChannel.Send(packetFactory!.Encode());
                var pubRelFactory = await Dispatcher.Instance.AddEventHandel(PacketType.PubRel);
                var pubRelPacket = (PubRelPacket)pubRelFactory!.GetPacket();
                var compPacket = new PubRecPacket()
                {
                    PacketIdentifier = pubRelPacket.PacketIdentifier
                };
                packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(compPacket);
                await _mqttChannel.Send(packetFactory!.Encode());
            }
        }
        else
        {
            Dispatcher.Instance.Dispatch(receivedPacket);     
        }
    }
    
  
    private void Ping(int second)
    {
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                var packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(new PingReqOption());
                await _mqttChannel.Send(packetFactory.Encode());
                await Dispatcher.Instance.AddEventHandel(PacketType.PingResp);
                
                await Task.Delay(second);
            }
        },  TaskCreationOptions.LongRunning);
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
        var packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(option);
        await _mqttChannel.Send(packetFactory!.Encode());
    }
    
    private async Task PublishAtLeastOnce(PublishOption option)
    {
            _packetIdentifierProvider.Next();
            while (_packetIdentifierHashSet.Contains(_packetIdentifierProvider.Current))
            {
                _packetIdentifierProvider.Next();
            }
            _packetIdentifierHashSet.Add(_packetIdentifierProvider.Current);
            option.PacketIdentifier = _packetIdentifierProvider.Current;            
            
            var packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(option);
            while (_packetIdentifierHashSet.Contains(option.PacketIdentifier))
            {
                try
                {
                    await _mqttChannel.Send(packetFactory!.Encode());
                    var pubAckFactory = await Dispatcher.Instance.AddEventHandel(PacketType.PubAck);
                    var  pubAckPacket = (PubAckPacket) pubAckFactory!.GetPacket();
                   
                    _packetIdentifierHashSet.Remove(pubAckPacket.PacketIdentifier);
                }
                catch
                {
                    var publishPack = (PublishPacket)packetFactory.GetPacket();
                    publishPack.Dup = true;
                }
            }
    }
    
    private async Task PublishExactlyOnce(PublishOption option)
    {
            _packetIdentifierProvider.Next();
            while (_packetIdentifierHashSet.Contains(_packetIdentifierProvider.Current))
            {
                _packetIdentifierProvider.Next();
            }
            _packetIdentifierHashSet.Add(_packetIdentifierProvider.Current);
            option.PacketIdentifier = _packetIdentifierProvider.Current;            
            
            var packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(option);
            while (_packetIdentifierHashSet.Contains(option.PacketIdentifier))
            {
                try
                {
                    await _mqttChannel.Send(packetFactory!.Encode());
                    var pubRecFactory = await Dispatcher.Instance.AddEventHandel(PacketType.PubRec);
                    var  pubRecPacket = (PubRecPacket) pubRecFactory!.GetPacket();

                    var pubRelPacket = new PubRelPacket()
                    {
                        PacketIdentifier = pubRecPacket.PacketIdentifier
                    };
                    var packetRelFactory = PacketFactory.PacketFactory.CreatePacketFactory(pubRelPacket);
                    await _mqttChannel.Send(packetRelFactory!.Encode());
                    var pubCompFactory = await Dispatcher.Instance.AddEventHandel(PacketType.PubComp);
                    var compPacket = (PubCompPacket)pubCompFactory.GetPacket();
                    _packetIdentifierHashSet.Remove(compPacket.PacketIdentifier);
                }
                catch
                {
                    var publishPack = (PublishPacket)packetFactory.GetPacket();
                    publishPack.Dup = true;
                }
            }
    }
    
    public async Task<SubAckOption> Subscribe(SubscribeOption option)
    {
        var packetFactory = PacketFactory.PacketFactory.CreatePacketFactory(option);
        
        await _mqttChannel.Send(packetFactory.Encode());
        packetFactory = await Dispatcher.Instance.AddEventHandel(PacketType.Subscribe);
        
        var subAckOption = (SubAckOption)packetFactory.GetOption();

        return subAckOption;
    }
    
    public void Dispose()
    {
        _mqttChannel.Dispose();
    }

}