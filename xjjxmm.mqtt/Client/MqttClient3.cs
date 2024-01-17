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

    private MqttPacketDispatcher _dispatcher;
    
    private async Task<T> Send<T>(IReceiveCommand command, int timeout = 1000 * 10)
    {
        _commands.Enqueue(command);
        await _mqttChannel.Send(command);
        
        using var cts = new CancellationTokenSource();
        cts.Token.Register(() => command.Result.TrySetException(new TimeoutException()), useSynchronizationContext: false);
        cts.CancelAfter(timeout);
        
        var received = await command.Result.Task;
        var option = command.Decode(received);
        return (T)option;
    }
    
    private async Task Send(ICommand command)
    {
        await _mqttChannel.Send(command);
    }
    
    public async Task<ConnAckOption> Connect(ConnectOption option)
    {
        _mqttChannel.Received = Receive;
        _dispatcher = new MqttPacketDispatcher();

        var command = Command.Command.Create(CommandEnum.SendConnect, option);
        
       // await _mqttChannel.Connect(option);
       // var command = new ConnectCommand(option);
        
        //var connAck = await Send<ConnAckOption>(command);

        /*if (connAck.ReasonCode != ConnectReturnCode.Accepted)
        {
            _mqttChannel.Dispose();
        }
        
       Ping(option.KeepAliveSecond * 1000);*/
       //ReceiveCommand();
       
        return connAck;
    }

    private ICommand FindCommand(PacketType packetType)
    {
        ICommand command;
        while (_commands.TryDequeue(out command))
        {
            if (command.PacketType == packetType)
            {
                return command;
            }
            else
            {
                _commands.Enqueue(command);
            }
        }

        throw new NotSupportedException();
    }
    
    private async Task Receive(ReceivedPacket receivedPacket)
    {
        var packetType = receivedPacket.GetPacketType();
        if (packetType == PacketType.Publish)
        {
            var packet = new PublishPacket().Decode(receivedPacket);
            var publishCommand = new PublisCommand(_mqttChannel, packet);
            publishCommand.S(receivedPacket);
        }

        await _dispatcher.Dispatch(receivedPacket);
    }
    
    private void ReceiveCommand()
    {
       
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                var command = new ReceivePublishCommand();
                var receivedPacket = await _mqttChannel.ReceiveCommand(command);
                var publishOption = (PublishOption) command.Decode(receivedPacket);
                if (publishOption.QoS == Qos.AtMostOnce)
                {
                    ReceiveMessage?.Invoke(publishOption.Message);
                }
                else if (publishOption.QoS == Qos.AtLeastOnce)
                {
                    var pubAckCommand = new PubAckCommand(new PubAckOption()
                    {
                        PacketIdentifier = publishOption.PacketIdentifier
                    });
                    
                    await _mqttChannel.SendNoAnswer(pubAckCommand);
                    
                    ReceiveMessage?.Invoke(publishOption.Message);
                }
                else if (publishOption.QoS == Qos.ExactlyOnce)
                {
                    var pubRecCommand = new PubRecCommand(new PubRecOption()
                    {
                        PacketIdentifier = publishOption.PacketIdentifier
                    });
                    
                    var pubRelOption = await Send<PubRelOption>(pubRecCommand);

                    var pubCompCommand = new PubCompCommand(new PubCompOption
                    {
                        PacketIdentifier = pubRelOption.PacketIdentifier
                    });
                  
                    await _mqttChannel.SendNoAnswer(pubCompCommand);
                }
            }
        },  TaskCreationOptions.LongRunning);
    }
    
    private void Ping(int second)
    {
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                var command = new PingReqCommand(new PingReqOption());
                await Send<PingRespOption>(command);
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
        var  command = new PublishAtMostOnceCommand(option);
        await Send(command);
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
    
    public void Dispose()
    {
        //socket.Close();
    }

}