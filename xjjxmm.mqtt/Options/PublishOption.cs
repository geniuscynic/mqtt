using mqtt.server.Constant;

namespace xjjxmm.mqtt.Options;

/// <summary>
/// 
/// </summary>
/// <param name="TopicName"></param>
/// <param name="Message"></param>
public class PublishOption : IOption
{
    public string TopicName { get; set; }
    public string Message { get; set; }
    
    //public bool Dup { get; set; } = false; //重发标志 DUP

    public byte QoS { get; set; } = Qos.AtMostOnce; //服务质量等级 QoS 

    //public bool Retain { get; set; } = false; //保留标志 RETAIN

    //public    ushort PacketIdentifier {get; set; }
    //报文标识符 Packet Identifier
    
}