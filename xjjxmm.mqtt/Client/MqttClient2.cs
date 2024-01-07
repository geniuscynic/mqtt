using mqtt.server.Constant;
using mqtt.server.Options;
using xjjxmm.mqtt.Channel;
using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Client;

public class MqttClient2 : IDisposable
{
    private readonly MqttChannel2 _mqttChannel = new();
    
    public Action<PublishOption>? ReceiveMessage{ get; set; }
    
   
    private HashSet<int> packetIdentifierHashSet = new();
    private PacketIdentifierProvider _packetIdentifierProvider = new ();
    
    public async Task<ConnAckOption> Connect(ConnectOption option)
    {
        await _mqttChannel.Connect(option);
        var command = new ConnectCommand(option);
        
        var buffer = await _mqttChannel.Send(command);
        var connAck = new ConnAckPacket(buffer).Decode();

        if (connAck.ReasonCode != ConnectReturnCode.Accepted)
        {
            _mqttChannel.Dispose();
        }
        
        Ping(option.KeepAliveSecond * 1000);

        return connAck;
    }
    
     
    private void Ping(int second)
    {
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                await _mqttChannel.PingReq(new PingReqOption());
                await Task.Delay(second);
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