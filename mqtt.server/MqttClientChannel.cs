using System.Net;
using System.Net.Sockets;
using mqtt.client.test;
using mqtt.server.Constant;
using mqtt.server.Options;
using mqtt.server.Packet;
using mqtt.server.Util;
using XjjXmm.Infrastructure.Common;

namespace mqtt.server;

internal class MqttClient : IDisposable
{
    private SocketClient _socketClient;

    public Action<ConnAckOption>? ConnAckAction { get; set; }

    public Action<SubAckOption>? SubAckAction{ get; set; }

    public Action<PingRespOption>? PingRespAction{ get; set; }

    public Action<PubAckOption>? PubAckAction{ get; set; }
    
    public Action<PubRecOption>? PubRecAction{ get; set; }
    
    public Action<PubRelOption>? PubRelAction{ get; set; }
    
    public Action<PubCompOption>? PubCompAction{ get; set; }
    
    public MqttClient()
    {
        _socketClient = new SocketClient();
        //创建与远程主机的连接
    }

    public async Task Connect(ConnectOption option)
    {
        await _socketClient.Connect(option.Host, option.Port);
        await _socketClient.Send(new ConnectPacket(option).Encode());

        Receive();
        
        while (true)
        {
            await PingReq(new PingReqOption());
            await Task.Delay(option.KeepAliveSecond);
        }
        
    }
    
    private void ReceiveConnAck(ReceivedPacket buffer)
    {
        var connAck = (ConnAckOption) new ConnAckPacket().Decode(buffer);

        if (connAck.ReasonCode != ConnectReturnCode.Accepted)
        {
            _socketClient.Close();
        }
        
        ConnAckAction?.Invoke(connAck);
    }
    
    public async Task Subscribe(SubscribeOption option)
    {
        await _socketClient.Send(new SubscribePacket(option).Encode());
    }
    
    public void ReceiveSuback(ReceivedPacket buffer)
    {
        var option = (SubAckOption) new SubAckPacket().Decode(buffer);
        
        SubAckAction?.Invoke(option);
    }

    private async Task PingReq(PingReqOption option)
    {
        await _socketClient.Send(new PingReqPacket(option).Encode());
    }
    
    private void ReceivePingResp(ReceivedPacket buffer)
    {
        var option = (PingRespOption) new PingRespPacket().Decode(buffer);
        PingRespAction?.Invoke(option);
    }

    public async Task Publish(PublishOption option)
    {
        await _socketClient.Send(new PublishPacket(option).Encode());
    }

    public async Task SendPubAck(PubAckOption option)
    {
        await _socketClient.Send(new PubAckPacket(option).Encode());        
    }
    
    public void ReceivePubAck(ReceivedPacket buffer)
    {
        var option = (PubAckOption) new PubAckPacket().Decode(buffer);
        PubAckAction?.Invoke(option);
    }

    public async Task SendPubRec(PubRecOption option)
    {
        await _socketClient.Send(new PubRecPacket(option).Encode());
    }
    
    public void ReceivePubRec(ReceivedPacket buffer)
    {
        var option = (PubRecOption) new PubRecPacket().Decode(buffer);
        PubRecAction?.Invoke(option);
    }
    
    public void ReceivePubRel(ReceivedPacket buffer)
    {
        var option = (PubRelOption) new PubRelPacket().Decode(buffer);
        PubRelAction?.Invoke(option);
    }
    
    
    public void ReceivePubComp(ReceivedPacket buffer)
    {
        var option = (PubCompOption) new PubCompPacket().Decode(buffer);
        PubCompAction?.Invoke(option);
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
                 if (buffer[0] == (PacketType.CONNACK << 4))
                 {
                     ReceiveConnAck(receivePacket);
                 }
                else if (buffer[0] == PacketType.SUBACK << 4 )
                {
                    ReceiveSuback(receivePacket);
                }
                 else if (buffer[0] == PacketType.PINGRESP << 4 )
                 {
                     ReceivePingResp(receivePacket);
                 }
                 else if (buffer[0] == PacketType.PUBLISH << 4 )
                 {
                     ReceivePubAck(receivePacket);
                 }
                 else if (buffer[0] == PacketType.PUBREC << 4 )
                 {
                     ReceivePubRec(receivePacket);
                 }
                 else if (buffer[0] == PacketType.PUBREL << 4 )
                 {
                     ReceivePubRel(receivePacket);
                 }
            }
            else
            {
                //Console.WriteLine($"{socketInfo.Id} 断开连接");
                break;
            }
        }
       
        
       
    }
    
  



   
    

   
   


    
    /*
    private async Task ReceiveMessage2(ArraySegment<byte> buffer)
    {
        var receiveData = new List<byte>();

        var bytes = buffer[1..];

        int len = 0;
        while (true)
        {
            if (receiveData.Count == 0)
            {
                len = Util.ComputeRemainingLength(bytes);
                if (len < 128)
                {
                    bytes = bytes[1..];
                }
                else if (len < 16384)
                {
                    bytes = bytes[2..];
                }
                else
                {
                    bytes = bytes[3..];
                }

                var subjectLength = (bytes[0] << 8) | bytes[1];
                len = len - subjectLength - 2;


                bytes = bytes.Slice(subjectLength + 2);
            }


            receiveData.AddRange(bytes.ToArray());

            if (receiveData.Count == len)
            {
                Console.WriteLine(receiveData.ToStrings());
                Console.WriteLine();

                //  string test = "乐山大佛你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗结尾乐山大佛你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗结尾乐山大佛你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗结尾";
                // var testByte = GetBytes(test);

                receiveData.Clear();
                break;
            }
            else
            {
                var count = await socket.ReceiveAsync(buffer, SocketFlags.None);
                bytes = buffer[0..count];
            }
        }
    }
    */


    //private async Task<Response<string>> ReceiveMessage(ArraySegment<byte> buffer, byte qos)
    //{
    //    ReceiveResponse receiveResponse = new ReceiveResponse(buffer, socket, qos);

    //    return await receiveResponse.Result();
    //}

    /*public async Task<ArraySegment<byte>> Receive()
    {
        var size = 512;
        var buff = new byte[size];
        ArraySegment<byte> buffer = new ArraySegment<byte>(buff);

        var count = await socket.ReceiveAsync(buffer, SocketFlags.None);

        return buffer[..count];
    }*/

  /*  public async Task Receive()
    {
        var size = 512;
        var buff = new byte[size];
        ArraySegment<byte> buffer = new ArraySegment<byte>(buff);

        while (true)
        {
            var count = await socket.ReceiveAsync(buffer, SocketFlags.None);
            if (count == 0)
            {
                continue;
            }

            var bit = buffer[0];
            var data = buffer[..count];
            switch (bit)
            {        
                case (PacketType.CONNACK << 4) | HeaderFlag.CONNACK:
                    ConnAckAction?.Invoke(await ReceiveConnack(data));
                    break;
                case (PacketType.SUBACK << 4) | HeaderFlag.SUBACK:
                    SubAckAction?.Invoke(await ReceiveSuback(data));
                    break;
                case PacketType.PUBLISH << 4:
                    ReceiveMessageAction?.Invoke(await ReceiveMessage(data, Qos.Qos0));

                    break;
                case PacketType.PUBLISH << 4 | Qos.Qos1 << 1:
                    ReceiveMessageAction?.Invoke(await ReceiveMessage(data, Qos.Qos1));

                    break;
                case PacketType.PUBLISH << 4 | Qos.Qos1 << 1 | 1 << 3:
                    ReceiveMessageAction?.Invoke(await ReceiveMessage(data, Qos.Qos1));

                    break;

                case PacketType.PUBLISH << 4 | Qos.Qos2 << 1:
                    ReceiveMessageAction?.Invoke(await ReceiveMessage(data, Qos.Qos2));

                    break;
                case PacketType.PUBLISH << 4 | Qos.Qos2 << 1 | 1 << 3:
                    ReceiveMessageAction?.Invoke(await ReceiveMessage(data, Qos.Qos2));

                    break;

                case (PacketType.PINGRESP << 4) | HeaderFlag.PINGRESP:
                    PingAckAction?.Invoke(ReceivePingResp(data));
                    break;

                case (PacketType.PUBACK << 4) | HeaderFlag.PUBACK:
                    await ReceivePubAck(data);
                    break;

                case (PacketType.PUBREC << 4) | HeaderFlag.PUBREC:
                    await ReceivePubRec(data);
                    break;

                case (PacketType.PUBCOMP << 4) | HeaderFlag.PUBCOMP:
                    await ReceivePubComp(data);
                    break;
            }
        }
    }
*/

    public async Task<dynamic> ReceiveMessage(ArraySegment<byte> buffer)
    {
        var receiveData = new List<byte>();

        var qos = (byte)(buffer[0] & 0x06) >> 1;

        var bytes = buffer[1..];

        int len = 0;

        int packetIdentifier = 0;

        while (true)
        {
            if (receiveData.Count == 0)
            {
                len = client.test.Util.ComputeRemainingLength(bytes);
                if (len < 128)
                {
                    bytes = bytes[1..];
                }
                else if (len < 16384)
                {
                    bytes = bytes[2..];
                }
                else
                {
                    bytes = bytes[3..];
                }

                var subjectLength = (bytes[0] << 8) | bytes[1];
                len = len - subjectLength - 2;

                bytes = bytes[(subjectLength + 2)..];

                if (qos > 0)
                {
                    len -= 2;
                    packetIdentifier = bytes[0] << 8 | bytes[1];

                    bytes = bytes[2..];
                }


                if (len < bytes.Count)
                {
                    bytes = bytes[0..len];
                }
            }


            receiveData.AddRange(bytes.ToArray());

            if (receiveData.Count == len)
            {
                //Console.WriteLine(receiveData.ToStrings());
                //Console.WriteLine();

                //  string test = "乐山大佛你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗结尾乐山大佛你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗结尾乐山大佛你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗你好吗结尾";
                // var testByte = GetBytes(test);

                //receiveData.Clear();

                //if (qos == Qos.Qos1)
                //{
                //    await SendPubAck(packetIdentifier);
                //}
                //else if (qos == Qos.Qos2)
                //{
                //    await SendPubRec(packetIdentifier);

                //    var count = await socket.ReceiveAsync(buffer, SocketFlags.None);
                //    bytes = buffer[0..count];

                //    var res = new PublishRelOutput(bytes);
                //    await SendPubComp(res.Result().Result);
                //}

                return new
                {
                    msg = receiveData.ToStrings(),
                    packetIdentifier
                };
            }
            else
            {
                var count = await socket.ReceiveAsync(buffer, SocketFlags.None);
                bytes = buffer[0..count];
            }
        }
    }



    internal async Task SendPubRec(int packetIdentifier)
    {
        var packet = new PublishRecInput(packetIdentifier);

        await socket.SendAsync(packet.ToBytes());
    }

    internal async Task SendPubComp(int packetIdentifier)
    {
        var packet = new PubCompPacket(packetIdentifier);

        await socket.SendAsync(packet.ToBytes());


    }

    public void Dispose()
    {
        socket.Close();
    }
}