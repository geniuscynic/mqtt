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
    private readonly SocketProxy _server = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private SocketIdProvider _provider = new ();


    private Dictionary<string, SocketInfo> _socketInfoDict = new();
    private Dictionary<string, List<string>> _subscribeDict = new();
    public async Task Start()
    {
        // _dispatcher = new Dispatcher();
        await ClientConnected();
    }

    private async Task ClientConnected()
    {
        //https://www.cnblogs.com/whuanle/p/10375526.html
        IPAddress iP = IPAddress.Parse("127.0.0.1");
        _server.Bind(new IPEndPoint(iP, 1883));
        _server.Listen(10);//开始监听
        
        while (true)
        {
            var client = await _server.Accept();
           
            MqttClientChannel clientChannel = new MqttClientChannel(client);
            

            SocketInfo socketInfo = new SocketInfo();
            socketInfo.Id = _provider.Next();
            socketInfo.Channel = clientChannel;
            clientChannel.ReceiveMessage = async (receivedPacket) => { await Receive(socketInfo, receivedPacket); };
            clientChannel.Init();
            await Task.Delay(100000);
        }
    }

    private async Task Receive(SocketInfo socketInfo, Packet.MqttPacket receivedPacket)
    {
        if (receivedPacket is ConnectPacket connectPacket)
        {
            socketInfo.ClientId = connectPacket.ClientId;
            AddSocketClient(socketInfo);
        }
        else if(receivedPacket is DisConnectPacket)
        {
            RemoveSocketClient(socketInfo);
        }
        else if (receivedPacket is SubscribePacket subscribePacket)
        {
            if (!_subscribeDict.ContainsKey(subscribePacket.TopicName))
            {
                _subscribeDict.Add(subscribePacket.TopicName, new List<string>());
            }

            if (!_subscribeDict[subscribePacket.TopicName].Contains(socketInfo.ClientId))
            {
                _subscribeDict[subscribePacket.TopicName].Add(socketInfo.ClientId);
            }
        }
        else if (receivedPacket is PingReqPacket)
        {
            
        }
        else if(receivedPacket is PublishPacket publishPacket)
        {
                if(_subscribeDict.TryGetValue(publishPacket.TopicName, out var socketIds))
                {
                    var missIds = new List<string>();
                    foreach (var socketId in socketIds)
                    {
                        if (_socketInfoDict.TryGetValue(socketId, out var subSocketInfo))
                        {
                           // var packetFactory = AdaptFactory.CreatePacketFactory(option);
                            await subSocketInfo.Channel.Publish(new PublishOption()
                            {
                                 TopicName = publishPacket.TopicName,
                                 Message = publishPacket.Message,
                                 QoS= publishPacket.QoS
                            });
                        }
                        else
                        {
                            missIds.Add(socketId);
                        }
                    }
                    socketIds.RemoveAll(t => missIds.Contains(t));
            }
        }
    }

    public void Dispose()
    {
        _server.Close();
    }

    private void AddSocketClient(SocketInfo socketInfo)
    {
        "new client Id".Dump();
        socketInfo.ClientId.Dump();
        //RemoveSocketClient(socketInfo);
        if (_socketInfoDict.TryGetValue(socketInfo.ClientId, out var existClient))
        {
            "Exist client id:".Dump();
            existClient.ClientId.Dump();
            existClient.Channel.Dispose();
        }
        _socketInfoDict[socketInfo.ClientId] = socketInfo;
    }

    private void RemoveSocketClient(SocketInfo socketInfo)
    {
        _socketInfoDict.Remove(socketInfo.ClientId);
    }
}

internal class SocketInfo
{
    public int Id { get; set; }
    public string ClientId { get; set; }
    //public Dispatcher Dispatcher { get; set; }
    public MqttClientChannel Channel { get; set; }
}