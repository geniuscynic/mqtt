using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PublishPacket : IdentifierPacket
{
    public string TopicName { get; set; }
    public string Message { get; set; }
    
    public bool Dup { get; set; } = false; //重发标志 DUP

    public byte QoS { get; set; } = Qos.AtMostOnce; //服务质量等级 QoS 

    public bool Retain { get; set; } = false; //保留标志 RETAIN
    
    public override string ToString()
    {
        return
            $"Publish: [Topic={TopicName}] [QoSLevel={QoS}] [Dup={Dup}] [Retain={Retain}] [PacketIdentifier={PacketIdentifier}]";
    }
}