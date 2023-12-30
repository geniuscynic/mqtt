﻿using mqtt.server;
using mqtt.server.Constant;
using mqtt.server.Options;
using mqtt.server.Packet;
using mqtt.server.Util;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Channel;

internal class MqttChannel : IDisposable
{
    private SocketClient _socketClient;

    private CancellationTokenSource _cancellationTokenSource = new();
    public Action<ConnAckOption>? ConnAckAction { get; set; }

    public Action<SubAckOption>? SubAckAction{ get; set; }

    public Action<PingRespOption>? PingRespAction{ get; set; }

    public Action<PubAckOption>? PubAckAction{ get; set; }
    
    public Func<PubRecOption, Task>? PubRecAction{ get; set; }
    
    public Action<PubRelOption>? PubRelAction{ get; set; }
    
    public Action<PubCompOption>? PubCompAction{ get; set; }
    
    public Action<UnSubAckOption>? UnSubAckAction{ get; set; }
    
    public MqttChannel()
    {
        _socketClient = new SocketClient();
        //创建与远程主机的连接
    }

    public async Task SendConnect(ConnectOption option)
    {
        await _socketClient.Connect(option.Host, option.Port);
        await _socketClient.Send(new ConnectPacket(option).Encode(), _cancellationTokenSource.Token);

        Receive();
    }
    
    private void ReceiveConnAck(ReceivedPacket buffer)
    {
        var connAck = (ConnAckOption) new ConnAckPacket(buffer).Decode();

        if (connAck.ReasonCode != ConnectReturnCode.Accepted)
        {
            _socketClient.Close();
        }
        
        ConnAckAction?.Invoke(connAck);
    }
    
    public async Task PingReq(PingReqOption option)
    {
        await _socketClient.Send(new PingReqPacket(option).Encode(), _cancellationTokenSource.Token);
    }
    
    public async Task SendPublish(PublishOption option)
    {
        await _socketClient.Send(new PublishPacket(option).Encode(), _cancellationTokenSource.Token);
    }

    public async Task SendPubAck(PubAckOption option)
    {
        await _socketClient.Send(new PubAckPacket(option).Encode(), _cancellationTokenSource.Token);        
    }
    
    public void ReceivePubAck(ReceivedPacket buffer)
    {
        var option = (PubAckOption) new PubAckPacket(buffer).Decode();
        PubAckAction?.Invoke(option);
    }

    public async Task SendPubRec(PubRecOption option)
    {
        await _socketClient.Send(new PubRecPacket(option).Encode(), _cancellationTokenSource.Token);
    }
    
    public async Task  ReceivePubRec(ReceivedPacket buffer)
    {
        var option = (PubRecOption) new PubRecPacket(buffer).Decode();
        if (PubRecAction != null)
        {
            await PubRecAction.Invoke(option);
        }
    }
    
    public async Task SendPubRel(PubRelOption option)
    {
        await _socketClient.Send(new PubRelPacket(option).Encode(), _cancellationTokenSource.Token);
    }
    
    public void ReceivePubRel(ReceivedPacket buffer)
    {
        var option = (PubRelOption) new PubRelPacket(buffer).Decode();
        PubRelAction?.Invoke(option);
    }
    
    public void ReceivePubComp(ReceivedPacket buffer)
    {
        var option = new PubCompPacket(buffer).Decode();
        PubCompAction?.Invoke(option);
    }
    
    private void ReceivePingResp(ReceivedPacket buffer)
    {
        var option = (PingRespOption) new PingRespPacket().Decode();
        PingRespAction?.Invoke(option);
    }
    

    
    public async Task Subscribe(SubscribeOption option)
    {
        await _socketClient.Send(new SubscribePacket(option).Encode(), _cancellationTokenSource.Token);
    }
    
    public void ReceiveSuback(ReceivedPacket buffer)
    {
        var option = (SubAckOption) new SubAckPacket(buffer).Decode();
        
        SubAckAction?.Invoke(option);
    }

    public async Task UnSubscribe(UnSubscribeOption option)
    {
        await _socketClient.Send(new UnSubscribePacket(option).Encode(), _cancellationTokenSource.Token);
    }
    
    public async Task ReceiveUnSubAck(UnSubAckOption option)
    {
        await _socketClient.Send(new UnSubAckPacket(option).Encode(), _cancellationTokenSource.Token);
        UnSubAckAction?.Invoke(option);
    }
    
    public void Dispose()
    {
        _socketClient.Close();
    }
    
    private async Task Receive()
    {
        const int bufferSize = 2;
        ArraySegment<byte> buffer = new (new byte[bufferSize]);

        while (true)
        {
            var size = await _socketClient.Receive(buffer); 
            if (size > 0)
            {
                var remainingLength = buffer[1];
                var body = await _socketClient.Receive(remainingLength);
                var receivePacket = new ReceivedPacket(buffer[0], remainingLength, body);
                 switch (buffer[0])
                 {
                     case PacketType.CONNACK << 4:
                         ReceiveConnAck(receivePacket);
                         break;
                     case PacketType.PUBACK << 4:
                         ReceivePubAck(receivePacket);
                         break;
                     case PacketType.PUBREC << 4:
                         ReceivePubRec(receivePacket);
                         break;
                     case PacketType.PUBREL << 4:
                         ReceivePubRel(receivePacket);
                         break;
                     case PacketType.PUBCOMP << 4:
                         ReceivePubComp(receivePacket);
                         break;
                     case PacketType.SUBACK << 4:
                         ReceiveSuback(receivePacket);
                         break;
                     case PacketType.PINGRESP << 4:
                         ReceivePingResp(receivePacket);
                         break;
                     case PacketType.PUBLISH << 4:
                         ReceivePubAck(receivePacket); //todo
                         break;
                    
                     case PacketType.UNSUBACK << 4:
                         ReceiveSuback(receivePacket);
                         break;
                 }
            }
            else
            {
                //Console.WriteLine($"{socketInfo.Id} 断开连接");
                break;
            }
        }
       
        
       
    }
    
  


}