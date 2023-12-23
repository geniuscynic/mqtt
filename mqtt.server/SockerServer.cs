/*
using System.Net;
using System.Net.Sockets;
using System.Text;
using mqtt.client.test.Mqtt;
using mqtt.server.Constant;
using mqtt.server.MqttPacket;
using mqtt.server.Util;

namespace mqtt.server.Client
{
    //https://zhuanlan.zhihu.com/p/653496155
    internal class SocketServer
    {
        private SocketProxy _server;

        private int _socketId = 0;
        private int _fixHeader = 2;
        private int _bufferSize = 1024;
        
        public SocketServer(string ip, int port)
        {
            _server = new SocketProxy();
            _server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _server.Listen(20);
            Console.WriteLine(string.Format("ServerSocket:{0}启动成功!", _server.LocalEndPoint));
        }

        public async Task Start()
        {
            await ClientConnected();
        }
        
        private async Task ClientConnected()
        {
            while (true)
            {
                var client = await _server.Accept();
                SocketInfo socketInfo = new SocketInfo();
                _socketId++;
                socketInfo.Id = _socketId;
                socketInfo.Socket = client;
                ReceiveMessage(socketInfo);
            }
        }

        private async Task ReceiveMessage(SocketInfo socketInfo)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[_fixHeader]);
            while (true)
            {
                var size = await socketInfo.Socket.Receive(buffer);
                if (size > 0)
                {
                    var msg = System.Text.Encoding.UTF8.GetString(buffer.ToArray(), 0, size);
                    Console.WriteLine($"接收到的数据：{socketInfo.Id}:{msg}");
                    Console.WriteLine($"{WriteArray(buffer)}");
                    
                    var remainingLength = buffer[1];
                    var body = await socketInfo.Socket.Receive(remainingLength);
                    var receivePacket = new ReceivedPacket(buffer[0], remainingLength, body);
                    Console.WriteLine($"body: {WriteArray(body)}");
                    if (buffer[0] == (ControlPacketType.Connect << 4))
                    {
                        var packet = DecodeConnect(receivePacket);
                        var sendPackets = EncodeConnAck(packet);
                        await socketInfo.Socket.Send(sendPackets);
                    }
                    else if (buffer[0] == (ControlPacketType.PINGREQ << 4))
                    {
                        DecodePing(receivePacket);
                        var sendPackets = EncodePingResp();
                        await socketInfo.Socket.Send(sendPackets);
                    }
                    else if (buffer[0] == ControlPacketType.SUBSCRIBE << 4 )
                    {
                        var packet = DecodeSubscribe(receivePacket);
                        var sendPackets = EncodeSubAck(packet);
                        await socketInfo.Socket.Send(sendPackets);
                    }
                    else if (buffer[0] == ControlPacketType.PUBLISH << 4)
                    {
                        var packet = DecodePublishPacket(receivePacket);
                        var sendPackets = EncodePubAck(packet);
                        await socketInfo.Socket.Send(sendPackets);
                    }
                }
                else
                {
                    Console.WriteLine($"{socketInfo.Id} 断开连接");
                    break;
                }
            }
        }

        private ConnectPacket DecodeConnect(ReceivedPacket receivePacket)
        {
                var packet = new ConnectPacket();
                var reader = new BufferReaderHelper(receivePacket.Body);
 
                var protocolName = reader.Next(6);
                if(!protocolName.SequenceEqual(new byte[] { 0x00, 0x04, 0x4d, 0x51, 0x54, 0x54}))
                {
                   
                }

                var protocolLevel = reader.Next();
                if (protocolLevel != 0x04)
                {
                    var response = ConnectReturnCode.ConnectionRefusedUnacceptableProtocolVersion;
                }
                
                var connectFlags = reader.Next();
                packet.CleanSession = (connectFlags & 0x2) > 0;
                packet.WillFlag = (connectFlags & 0x4) > 0;
                packet.WillQoS = (byte)((connectFlags & 0x18) >> 3);
                packet.WillRetain = (connectFlags & 0x20) > 0;
                var passwordFlag = (connectFlags & 0x40) > 0;
                var usernameFlag = (connectFlags & 0x80) > 0;
                
                packet.KeepAlivePeriod = reader.NextTwoByteInt();
                
                //payload
                var clientLength =reader.NextTwoByteInt();
                packet.ClientId = reader.NextStr(clientLength);
                
               // if (willFlag)
               // {
                    //packet.WillTopic = _bufferReader.ReadString();
                   // packet.WillMessage = _bufferReader.ReadBinaryData();
               // }
                
                
                if (usernameFlag)
                {
                    //packet.Username = _bufferReader.ReadString();
                }

                if (passwordFlag)
                {
                    //packet.Password = _bufferReader.ReadBinaryData();
                }

                return packet;
        }

        private void DecodePing(ReceivedPacket receivedPacket)
        {
            
        }
        
        private ArraySegment<byte> EncodeConnAck(ConnectPacket connectPacket)
        {
            return new ConnectAckPacket().Build();
        }
        
        private ArraySegment<byte> EncodePingResp()
        {
            return new PingRespPacket().Build();
        }
        
        private SubscribePacket DecodeSubscribe(ReceivedPacket receivedPacket)
        {
            var rest = receivedPacket.Body;
            var msb = rest[0];
            var lsb = rest[1];
            var packetIdentifier = ((msb << 8) | lsb);
            
            var topicMsb =  rest[2];
            var topicLsb = rest[3];
            var topicLength = ((topicMsb << 8) | topicLsb);
            var topicByte = rest[4..(4+topicLength)];
            var topic = Encoding.UTF8.GetString(topicByte.ToArray(), 0, topicByte.Count);     
            Console.WriteLine($"Subscribe to {topic}");

            SubscribePacket packet = new SubscribePacket()
            {
                Identifier = packetIdentifier,
                TopicFilters = new List<TopicFilter>() { new TopicFilter() { Qos = Qos.AtMostOnce, Topic = topic } }
            };
            
            return packet;
        }
        
        private ArraySegment<byte> EncodeSubAck(SubscribePacket subscribePacket)
        {
            return new SubAckPacket()
            {
                PacketIdentifier = subscribePacket.Identifier,
                ReasonCodes = new List<SubscribeReasonCode>() { SubscribeReasonCode.GrantedQoS0 }
            }.Build();
        }


        private PublishPacket DecodePublishPacket(ReceivedPacket receivedPacket)
        {
            PublishPacket packet = new PublishPacket();
            var helper = new BufferReaderHelper(receivedPacket.Body);
            var  identifier = helper.NextTwoByteInt();
            packet.Topic = helper.NextStr(identifier);  
            
            return packet;
        }

        private ArraySegment<byte> EncodePubAck(PublishPacket publishPacket)
        {
            return new ArraySegment<byte>();
        }
        
        private string WriteArray(ArraySegment<byte> buffer)
        {
            var sb = new StringBuilder();
            foreach (var b in buffer)
            {
                sb.Append(b);
                sb.Append(" ");
            }

            return sb.ToString();
        }
    }
    
}
*/
