using System.Collections.Concurrent;
using mqtt.server.Constant;
using mqtt.server.Options;
using xjjxmm.mqtt.Channel;
using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.Client;

public class MqttClient3 : IDisposable
{
    private readonly MqttChannel3 _mqttChannel = new();
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
            //todo
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
                    await _mqttChannel.Send(packetFactory!.Encode());
                    
                    var comOption = await Send<PubCompOption>(pubRelCommand);
                    _packetIdentifierHashSet.Remove(comOption.PacketIdentifier);
                }
                catch
                {
                    option.Dup = true;
                    command = new PublishExactlyOnceCommand(option);
                }
            }
    }
    
    public async Task<SubAckOption> Subscribe(SubscribeOption option)
    {
        var packetFactory = PacketFactory.CreatePacketFactory(option);
        
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