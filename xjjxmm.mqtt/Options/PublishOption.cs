using mqtt.server.Constant;

namespace mqtt.server.Options;

/// <summary>
/// todo: 缺少报文标识符 Packet Identifier
/// </summary>
/// <param name="TopicName"></param>
/// <param name="Message"></param>
public record PublishOption(string TopicName, string Message)
{
    public bool Dup { get; set; } = false; //重发标志 DUP

    public byte QoS { get; set; } = Qos.AtMostOnce; //服务质量等级 QoS 

    public bool Retain { get; set; } = false; //保留标志 RETAIN

    public    int PacketIdentifier {get; set; }
    //报文标识符 Packet Identifier

    public override string ToString()
    {
        return
            $"Publish: [Topic={TopicName}] [QoSLevel={QoS}] [Dup={Dup}] [Retain={Retain}] [PacketIdentifier={PacketIdentifier}]";
    }
}