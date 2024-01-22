using mqtt.server.Constant;

namespace xjjxmm.mqtt.Options;

public record class SubscribeOption(string TopicName) : IOption
{
   public byte QoS { get; set; } = Qos.AtMostOnce; //服务质量等级 QoS  
}