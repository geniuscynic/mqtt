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
    
   
    private readonly HashSet<int> _packetIdentifierHashSet = new();
    private readonly PacketIdentifierProvider _packetIdentifierProvider = new ();

    private async Task<T> Send<T>(ICommand command)
    {

                var buffer = await _mqttChannel.Send(command);
                var option = command.Decode(buffer);
                return (T)option;

    }
   
    
    public async Task<ConnAckOption> Connect(ConnectOption option)
    {
        await _mqttChannel.Connect(option);
        var command = new ConnectCommand(option);


        var connAck = await Send<ConnAckOption>(command);

        if (connAck.ReasonCode != ConnectReturnCode.Accepted)
        {
            _mqttChannel.Dispose();
        }
        
        Ping(option.KeepAliveSecond * 1000);

        return connAck;
    }
    
    private void Ping(int second)
    {
        var command = new PingReqCommand(new PingReqOption());
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                await _mqttChannel.Send(command);
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
        var  command = new PublishAtMostOnceCommand(option);
        await _mqttChannel.Send(command);
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
            
            var  command = new PublishAtLeastOnceCommand(option);
            while (_packetIdentifierHashSet.Contains(option.PacketIdentifier))
            {
                try
                {
                    var pubAckOption = await Send<PubAckOption>(command);
                    _packetIdentifierHashSet.Remove(pubAckOption.PacketIdentifier);
                }
                catch
                {
                    option.Dup = true;
                    command = new PublishAtLeastOnceCommand(option);
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
            
            var  command = new PublishExactlyOnceCommand(option);
           
      
            while (_packetIdentifierHashSet.Contains(option.PacketIdentifier))
            {
                try
                {
                    var pubRecOption = await Send<PubRecOption>(command);
                    //_packetIdentifierHashSet.Remove(pubAckOption.PacketIdentifier);
                    
                    var pubRelCommand = new PubRelCommand(new PubRelOption()
                    {
                        PacketIdentifier = pubRecOption.PacketIdentifier
                    });
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
        var command = new SubscribeCommand(option);
        return await Send<SubAckOption>(command);
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