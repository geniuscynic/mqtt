﻿using System.Net;
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
        IPAddress iP = IPAddress.Parse("127.0.0.1");
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
        if (receivedPacket is ConnectPacket connectPacket)
        {
            socketInfo.ClientId = connectPacket.ClientId;
            AddSocketClient(socketInfo);
        }
        else if (receivedPacket is DisConnectPacket)
        {
            RemoveSocketClient(socketInfo);
        }
        else if (receivedPacket is SubscribePacket subscribePacket)
        {
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
            foreach (var topicFilter in unSubscribePacket.TopicNames)
            {
                _subscribeDict[topicFilter].Remove(socketInfo);
                socketInfo.SubscribeInfos.Remove(topicFilter);
            }
        }
        else if (receivedPacket is PingReqPacket)
        {
        }
        else if (receivedPacket is PublishPacket publishPacket)
        {
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
            
            publishPacket.Message.Substring(0, 10).Dump();
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

internal class SocketInfo
{
    public int Id { get; set; }

    public string ClientId { get; set; }

    //public Dispatcher Dispatcher { get; set; }
    public MqttClientChannel Channel { get; set; }

    public Dictionary<string, byte> SubscribeInfos { get; set; } = new();
}

internal class SubscribeInfo
{
    public string TopicName { get; set; }

    public byte QoS { get; set; }
}