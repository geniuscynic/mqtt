using mqtt.client.test;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class ConnectPacketAdapt : IAdaptFactory
{
    private readonly ConnectPacket packet;

    public ConnectPacketAdapt(ConnectOption option)
    {
        packet = CreatePacket(option);
    }
    
    public ConnectPacketAdapt(ReceivedPacket received)
    {
        packet = new ConnectPacket();
        
        var helper = received.GetPacketHelper();
        var remainingLength = received.RemainingLength;

       
        helper.NextStr();

        var protocol = helper.Next();
        var connectFlags = helper.Next();
        packet.CleanSession = (connectFlags & 0b_0000_0010) == 0b_0000_0010;
        var willFlag = (connectFlags & 0b_0000_0100) == 0b_0000_0100;
        packet.QoS = (byte) ((connectFlags & 0b_0000_1100) >> 3);
        packet.Retain = (connectFlags & 0b_0010_0000) == 0b_0010_0000;
        
        var hasUser = (connectFlags & 0b_0100_0000) == 0b_0100_0000;
        var hasPassword = (connectFlags & 0b_1000_0000) == 0b_1000_0000;
        packet.KeepAliveSecond = helper.NextTwoByteInt();
        
        var clientId = helper.NextStr();
        packet.ClientId = clientId;
        if (willFlag)
        {
            packet.WillTopic = helper.NextStr();
            packet.WillMessage = helper.NextStr();
        }

        if (hasUser)
        {
            packet.UserName = helper.NextStr();
        }

        if (hasPassword)
        {
            packet.Password = helper.NextStr();
        }
    }
    
    public ConnectPacketAdapt(ConnectPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    public IOption GetOption()
    {
        return new ConnectOption(packet.Host, packet.Port, packet.ClientId);
    }

    public ArraySegment<byte> Encode()
    {
        return CreateBytes(packet);
    }
    
    private ConnectPacket CreatePacket(ConnectOption option)
    {
        ConnectPacket packet = new ConnectPacket
        {
            Host = option.Host,
            Port = option.Port,
            ClientId = option.ClientId,
            ProtocolLevel = option.ProtocolLevel,
            CleanSession = option.CleanSession,
            WillTopic = option.WillTopic,
            WillMessage = option.WillMessage,
            QoS = option.QoS,
            Retain = option.Retain,
            UserName = option.UserName,
            Password = option.Password,
            KeepAliveSecond = option.KeepAliveSecond
        };
        return packet;
    }
    
    private byte[] _clientByte;
    private ConnectPacket _connectOption;
    private byte[]? _passwordByte;
    private byte[]? _userNameByte;

    private byte[]? _willTopicByte;
    private byte[]? _willMessageByte;
    
    private List<byte> Data { get; } = new List<byte>();
    
    private ArraySegment<byte> CreateBytes(ConnectPacket connectOption)
    {
        _connectOption = connectOption;

        _clientByte = connectOption.ClientId.ToBytes();

        if (connectOption.WillFlag)
        {
            _willTopicByte = connectOption.WillTopic.ToBytes();
            _willMessageByte = connectOption.WillMessage.ToBytes();
        }
        
        if (connectOption.HasUserName) _userNameByte = connectOption.UserName.ToBytes();

        if (connectOption.HasPassword) _passwordByte = connectOption.Password.ToBytes();
        
        PushHeaders();
        PushRemainingLength();
        PushVariableHeader();
        PushPayload();
        return Data.ToArray();
    }

 
    
    private void PushHeaders()
    {
        Data.Add((short)ControlPacketType.Connect << 4);
    }

    private void PushRemainingLength()
    {
        var len = 10 + 2 + _clientByte.Length;
        
        if (_willTopicByte != null)
        {
            len += 2 + _willTopicByte.Length;
        }
        
        if (_willMessageByte != null)
        {
            len += 2 + _willMessageByte.Length;
        }
        
        if (_userNameByte != null)
        {
            len += 2 + _userNameByte.Length;
        }

        if (_passwordByte != null)
        {
            len += 2 + _passwordByte.Length;
        }

        foreach (var l in UtilHelpers.ComputeRemainingLength(len)) Data.Add(Convert.ToByte(l));
    }

    private void PushVariableHeader()
    {
        //协议名 Protocol Name
        Data.Add(0x00);
        Data.Add(0x04);
        Data.AddRange("MQTT".ToBytes());

        //协议级别 Protocol Level
        Data.Add(_connectOption.ProtocolLevel);

        //Connect Flags
        var conectFlags = 0x0;
        if (_connectOption.HasUserName) conectFlags |= 1 << 7;

        if (_connectOption.HasPassword) conectFlags |= 1 << 6;

        if (_connectOption.Retain) conectFlags |= 1 << 5;

        conectFlags |= _connectOption.QoS << 3;

        if (_connectOption.WillFlag) conectFlags |= 1 << 2;

        if (_connectOption.CleanSession) conectFlags |= 1 << 1;
        Data.Add(Convert.ToByte(conectFlags));

        //保持连接 Keep Alive
        Data.Add((byte)(_connectOption.KeepAliveSecond >> 8)); //心跳值
        Data.Add((byte)(_connectOption.KeepAliveSecond & 255));
    }

    private void PushPayload()
    {
        //有效载荷
        //CONNECT 报文的有效载荷（payload）包含一个或多个以长度为前缀的字段，可变报头中的标志决定是否包含这些字段。如果包含的话，必须按这个顺序出现：客户端标识符，遗嘱主题，遗嘱消息，用户名，密码。
        Data.Add((byte)(_clientByte.Length >> 8));
        Data.Add((byte)(_clientByte.Length & 255));
        Data.AddRange(_clientByte);

        if (_connectOption.WillFlag)
        {
            Data.Add((byte)(_willTopicByte!.Length >> 8));
            Data.Add((byte)(_willTopicByte.Length & 255));
            Data.AddRange(_willTopicByte!);
            
            Data.Add((byte)(_willMessageByte!.Length >> 8));
            Data.Add((byte)(_willMessageByte.Length & 255));
            Data.AddRange(_willMessageByte!);
        }
        
        if (_connectOption.HasUserName)
        {
            Data.Add((byte)(_userNameByte!.Length >> 8));
            Data.Add((byte)(_userNameByte.Length & 255));
            Data.AddRange(_userNameByte!);
        }

        if (_connectOption.HasPassword)
        {
            Data.Add((byte)(_passwordByte!.Length >> 8));
            Data.Add((byte)(_passwordByte.Length & 255));
            Data.AddRange(_passwordByte);
        }
    }
}