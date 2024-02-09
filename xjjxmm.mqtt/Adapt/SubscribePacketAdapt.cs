using mqtt.client.test;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class SubscribePacketAdapt : IAdaptFactory
{
    private readonly SubscribePacket packet;
    public SubscribePacketAdapt(SubscribeOption option,ushort packetIdentifier)
    {
        packet = new SubscribePacket()
        {
            TopicName = option.TopicName,
            QoS = option.QoS,
            PacketIdentifier = packetIdentifier
        };
    }
    
    public SubscribePacketAdapt(ReceivedPacket received)
    {
        packet = new SubscribePacket();
        
        var helper = received.GetPacketHelper();
        var remainingLength = received.RemainingLength;

        packet.PacketIdentifier = helper.NextTwoByteInt();
        packet.TopicName = helper.NextStr();
        packet.QoS =  helper.Next();
    }
    
    public SubscribePacketAdapt(SubscribePacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    private List<byte> Data { get; } = new List<byte>();
    
    private byte[] _subjectByte;
   
    protected void PushHeaders()
    {
        //固定报头
        Data.Add((byte)ControlPacketType.Subscribe << 4);
    }

    protected void PushRemainingLength()
    {
        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        var len = 2 + 2 + _subjectByte.Length + 1;

        foreach (var l in UtilHelpers.ComputeRemainingLength(len)) Data.Add(Convert.ToByte(l));
    }

    protected void PushVariableHeader()
    {
        var msb = (byte)(packet.PacketIdentifier >> 8);
        var lsb = (byte)(packet.PacketIdentifier & 255);
        Data.Add(msb);
        Data.Add(lsb);
    }

    protected void PushPayload()
    {
        //有效载荷
        //CONNECT 报文的有效载荷（payload）包含一个或多个以长度为前缀的字段，可变报头中的标志决定是否包含这些字段。如果包含的话，必须按这个顺序出现：客户端标识符，遗嘱主题，遗嘱消息，用户名，密码。
        Data.Add((byte)(_subjectByte.Length >> 8));
        Data.Add((byte)(_subjectByte.Length & 255));
        Data.AddRange(_subjectByte);

        Data.Add(packet.QoS);
    }
    
    public ArraySegment<byte> Encode()
    {
        
        _subjectByte = packet.TopicName.ToBytes();
        
        PushHeaders();
        PushRemainingLength();
        PushVariableHeader();
        PushPayload();
        return Data.ToArray();
    }

    public IOption GetOption()
    {
        return new SubscribeOption(packet.TopicName)
        {
            QoS = packet.QoS
        };
    }
}