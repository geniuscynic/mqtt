using System.Net;
using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Adapt;
using xjjxmm.mqtt.Client;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Server;

public class MqttServer : IDisposable
{
    public Func<AuthModel, Task<bool>>? ConnectAction { get; set; }
    
    private readonly SocketProxy _server = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private SocketIdProvider _provider = new();
    
    private Dictionary<string, SocketInfo> _socketInfoDict = new();
    private Dictionary<string, List<SocketInfo?>> _subscribeDict = new();

    public async Task Start()
    {
        // _dispatcher = new Dispatcher();
        await ClientConnected();
    }

    private async Task ClientConnected()
    {
        //https://www.cnblogs.com/whuanle/p/10375526.html
        IPAddress iP = IPAddress.Parse("192.168.88.155");
        _server.Bind(new IPEndPoint(iP, 1883));
        _server.Listen(10); //开始监听

        while (true)
        {
            var client = await _server.Accept();

            MqttClientChannel clientChannel = new MqttClientChannel(client);

            SocketInfo socketInfo = new SocketInfo();
            socketInfo.Id = _provider.Next();
            socketInfo.Channel = clientChannel;
            clientChannel.ReceiveMessage = async (receivedPacket) => { await Receive(socketInfo, receivedPacket); };
            clientChannel.StartReceive();
            // await Task.Delay(100000);
        }
    }

    private async Task Receive(SocketInfo socketInfo, Packet.MqttPacket receivedPacket)
    {
        if (receivedPacket is ConnectPacket connPacket)
        {
            await OnConnect(socketInfo, connPacket);
        }
        else if (receivedPacket is DisConnectPacket)
        {
            RemoveSocketClient(socketInfo);
        }
        else if (receivedPacket is SubscribePacket subscribePacket)
        {
            socketInfo.LastLiveTime = DateTime.Now;
            if (!_subscribeDict.ContainsKey(subscribePacket.TopicName))
            {
                _subscribeDict.Add(subscribePacket.TopicName, new List<SocketInfo>());
            }

            if (!_subscribeDict[subscribePacket.TopicName].Contains(socketInfo))
            {
                _subscribeDict[subscribePacket.TopicName].Add(socketInfo);
                socketInfo.SubscribeInfos[subscribePacket.TopicName] = subscribePacket.QoS;
            }
        }
        else if (receivedPacket is UnSubscribePacket unSubscribePacket)
        {
            socketInfo.LastLiveTime = DateTime.Now;
            foreach (var topicFilter in unSubscribePacket.TopicNames)
            {
                _subscribeDict[topicFilter].Remove(socketInfo);
                socketInfo.SubscribeInfos.Remove(topicFilter);
            }
        }
        else if (receivedPacket is PingReqPacket)
        {
            socketInfo.LastLiveTime = DateTime.Now;
        }
        else if (receivedPacket is PublishPacket publishPacket)
        {
            socketInfo.LastLiveTime = DateTime.Now;
            if (_subscribeDict.TryGetValue(publishPacket.TopicName, out var socketIds))
            {
                socketIds.RemoveAll(t => t == null);
                foreach (var subSocketInfo in socketIds)
                {
                    await subSocketInfo!.Channel.Publish(new PublishOption()
                    {
                        TopicName = publishPacket.TopicName,
                        Message = publishPacket.Message,
                        QoS = subSocketInfo.SubscribeInfos[publishPacket.TopicName]
                    });
                }
            }
            
            //publishPacket.Message.Substring(0, 10).Dump();
        }
    }

    private async Task OnConnect(SocketInfo socketInfo, ConnectPacket connPacket)
    {
        var connAckPacket = new ConnAckPacket()
        {
            ReasonCode = ConnectReturnCode.Accepted
        };
        if (connPacket.ProtocolLevel == MqttProtocolLevel.V311
            || connPacket.ProtocolLevel == MqttProtocolLevel.V5)
        {
            if (connPacket.HasPassword && connPacket.HasPassword)
            {
                if (ConnectAction != null)
                {
                    var isAuth = await ConnectAction(new AuthModel
                    {
                        UserName = connPacket!.UserName!,
                        Password = connPacket.Password!
                    });

                    if (!isAuth)
                    {
                        var packetFactory = AdaptFactory.CreatePacketFactory(connAckPacket);
                        await socketInfo.Channel.Send(packetFactory!);
                        socketInfo.Channel.Dispose();
                    }
                    else
                    {
                        var packetFactory = AdaptFactory.CreatePacketFactory(connAckPacket);
                        await socketInfo.Channel.Send(packetFactory!);
                        socketInfo.LastLiveTime = DateTime.Now;
                        socketInfo.ClientId = connPacket.ClientId;
                        AddSocketClient(socketInfo);
                    }
                }
            }
            else
            {
                var packetFactory = AdaptFactory.CreatePacketFactory(connAckPacket);
                await socketInfo.Channel.Send(packetFactory!);

                socketInfo.LastLiveTime = DateTime.Now;
                socketInfo.ClientId = connPacket.ClientId;
                AddSocketClient(socketInfo);
            }
        }
        else
        {
            connAckPacket.ReasonCode = ConnectReturnCode.UnsupportedProtocolVersion;
            var packetFactory = AdaptFactory.CreatePacketFactory(connAckPacket);
            await socketInfo.Channel.Send(packetFactory!);
            socketInfo.Channel.Dispose();
        }
    }

    public void Dispose()
    {
        _server.Close();
    }

    private void AddSocketClient(SocketInfo socketInfo)
    {
        //  "new client Id".Dump();
        //  socketInfo.ClientId.Dump();
        //RemoveSocketClient(socketInfo);
        if (_socketInfoDict.TryGetValue(socketInfo.ClientId, out var existClient))
        {
            // "Exist client id:".Dump();
            //existClient.ClientId.Dump();
            existClient.Channel.Dispose();
            existClient = null;
        }

        _socketInfoDict[socketInfo.ClientId] = socketInfo;
    }

    private void RemoveSocketClient(SocketInfo socketInfo)
    {
        _socketInfoDict.Remove(socketInfo.ClientId);
        socketInfo = null;
    }
}