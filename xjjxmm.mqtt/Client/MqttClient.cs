using mqtt.server;
using mqtt.server.Constant;
using mqtt.server.Options;
using xjjxmm.mqtt.Channel;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt;

public class MqttClient : IDisposable
{
    private readonly MqttChannel _mqttChannel = new();
    
    public Action<ConnAckOption>? ConnAckAction { get; set; }

    public Action<SubAckOption>? SubAckAction{ get; set; }

    public Action<PublishOption>? ReceiveMessage{ get; set; }
    
    //public Action<PingRespOption>? PingRespAction{ get; set; }

   // public Action<PubAckOption>? PubAckAction{ get; set; }
    
   // public Action<PubRecOption>? PubRecAction{ get; set; }
    
   // public Action<PubRelOption>? PubRelAction{ get; set; }
    
   // public Action<PubCompOption>? PubCompAction{ get; set; }
    
   // public Action<UnSubAckOption>? UnSubAckAction{ get; set; }

    //private int publicIdentifier = 1;
    private HashSet<int> packetIdentifierHashSet = new();
    private PacketIdentifierProvider _packetIdentifierProvider = new ();
    
    public async Task Connect(ConnectOption option)
    {
        _mqttChannel.ConnAckAction = ConnAckAction;
        _mqttChannel.PingRespAction = (option)=> Console.WriteLine( option.ToString());
        _mqttChannel.SubAckAction = SubAckAction;
        _mqttChannel.PublishAction = PublishAction;
        
            
        await _mqttChannel.SendConnect(option);

        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                await _mqttChannel.PingReq(new PingReqOption());
                await Task.Delay(option.KeepAliveSecond * 1000);
            }
        },  TaskCreationOptions.LongRunning);
    }
    
    public async Task Publish(PublishOption option)
    {
        switch (option.QoS)
        {
            case Qos.AtLeastOnce:
                await PublishAtLeastOnce(option);
                break;
            case Qos.AtMostOnce:
                await PublishAtMostOnce(option);
                break;
            case Qos.ExactlyOnce:
                await PublishExactlyOnce(option);
                break;
        }
    }

    private async Task PublishAtMostOnce(PublishOption option)
    {
        await _mqttChannel.SendPublish(option);
    }
    
    private async Task PublishAtLeastOnce(PublishOption option)
    {
            _packetIdentifierProvider.Next();
            while (packetIdentifierHashSet.Contains(_packetIdentifierProvider.Current))
            {
                _packetIdentifierProvider.Next();
            }
            packetIdentifierHashSet.Add(_packetIdentifierProvider.Current);
            option.PacketIdentifier = _packetIdentifierProvider.Current;            
            
        await _mqttChannel.SendPublish(option);
        
            _mqttChannel.PubAckAction = ackOption =>
            {
                packetIdentifierHashSet.Remove(ackOption.PacketIdentifier);
            };
            
            await Task.Delay(1000);
            // 重发
            while (packetIdentifierHashSet.Contains(option.PacketIdentifier))
            {
                option.Dup = true;
                await _mqttChannel.SendPublish(option);
                await Task.Delay(1000);
            }
    }
    
    private async Task PublishExactlyOnce(PublishOption option)
    {
        
            _packetIdentifierProvider.Next();
            while (packetIdentifierHashSet.Contains(_packetIdentifierProvider.Current))
            {
                _packetIdentifierProvider.Next();
            }
            packetIdentifierHashSet.Add(_packetIdentifierProvider.Current);
            option.PacketIdentifier = _packetIdentifierProvider.Current;            
            
        await _mqttChannel.SendPublish(option);

        var packetIdentifier = 0;
            _mqttChannel.PubRecAction = async ackOption =>
            {
                packetIdentifier = ackOption.PacketIdentifier;
                //packetIdentifierHashSet.Remove(ackOption.PacketIdentifier);
                await _mqttChannel.SendPubRel(new PubRelOption()
                {
                    PacketIdentifier = ackOption.PacketIdentifier
                });

            };
            
            _mqttChannel.PubCompAction = comOption =>
            {
                //packetIdentifierHashSet.Remove(ackOption.PacketIdentifier);
                packetIdentifierHashSet.Remove(comOption.PacketIdentifier);

            };
        
            await Task.Delay(1000);
            // 重发
            while (packetIdentifier == 0)
            {
                option.Dup = true;
                await _mqttChannel.SendPublish(option);
                await Task.Delay(1000);
            }
            
            while (packetIdentifierHashSet.Contains(option.PacketIdentifier))
            {
                await _mqttChannel.SendPubRel(new PubRelOption()
                {
                    PacketIdentifier = packetIdentifier
                });
                await Task.Delay(1000);
            }
    }
    
    public async Task Subscribe(SubscribeOption option)
    {
        await _mqttChannel.Subscribe(option);
    }

    private async Task PublishAction(PublishOption option)
    {
        if (option.QoS == Qos.AtMostOnce)
        {
            ReceiveMessage?.Invoke(option);
        }
        else if (option.QoS == Qos.AtLeastOnce)
        {
            await _mqttChannel.SendPubAck(new PubAckOption()
            {
                PacketIdentifier = option.PacketIdentifier
            });
            ReceiveMessage?.Invoke(option);
        }
        else if (option.QoS == Qos.AtMostOnce)
        {
            await _mqttChannel.SendPubRec(new PubRecOption()
            {
                PacketIdentifier = option.PacketIdentifier
            });

            _mqttChannel.PubRelAction = async relOption =>
            {
                await _mqttChannel.SendPubComp(new PubCompOption()
                {
                    PacketIdentifier = relOption.PacketIdentifier
                });
            };
        }
    }
    
    public void Dispose()
    {
        //socket.Close();
    }

}