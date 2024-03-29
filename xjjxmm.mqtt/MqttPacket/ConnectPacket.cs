﻿using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class ConnectPacket : AbstractDataPacket<ConnectOption>
{
    private readonly byte[] _clientByte;
    private readonly ConnectOption _connectOption;
    private readonly byte[]? _passwordByte;
    private readonly byte[]? _userNameByte;

    public ConnectPacket(ConnectOption connectOption)
    {
        _connectOption = connectOption;

        _clientByte = connectOption.ClientId.ToBytes();

        if (connectOption.HasUserName) _userNameByte = connectOption.UserName.ToBytes();

        if (connectOption.HasPassword) _passwordByte = connectOption.Password.ToBytes();
    }

    protected override void PushHeaders()
    {
        Data.Add(PacketType.CONNECT << 4);
    }

    protected override void PushRemainingLength()
    {
        var len = 10 + 2 + _clientByte.Length;
        if (_userNameByte != null) len += 2 + _userNameByte.Length;

        if (_passwordByte != null) len += 2 + _passwordByte.Length;

        foreach (var l in UtilHelpers.ComputeRemainingLength(len)) Data.Add(Convert.ToByte(l));
    }

    protected override void PushVariableHeader()
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

    protected override void PushPayload()
    {
        //有效载荷
        //CONNECT 报文的有效载荷（payload）包含一个或多个以长度为前缀的字段，可变报头中的标志决定是否包含这些字段。如果包含的话，必须按这个顺序出现：客户端标识符，遗嘱主题，遗嘱消息，用户名，密码。
        Data.Add((byte)(_clientByte.Length >> 8));
        Data.Add((byte)(_clientByte.Length & 255));
        Data.AddRange(_clientByte);

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

    public override ConnectOption Decode()
    {
        throw new NotImplementedException();
    }
}