using mqtt.server.Constant;
using mqtt.server.Options;

namespace xjjxmm.mqtt.Options
{
    public record ConnectOption(string Host, int Port, string ClientId) : IOption
    {
        
        public byte ProtocolLevel { get; set; } = Protocol.Level3; //协议级别 

        public bool CleanSession { get; set; } = true; //清理会话

        public bool WillFlag => !string.IsNullOrEmpty(WillTopic);

        public string WillTopic { get; set; } = string.Empty; //遗嘱主题

        public string WillMessage { get; set; } = string.Empty; //遗嘱消息

        public byte QoS { get; set; } = Qos.AtMostOnce; //QoS 

        public bool Retain { get; set; } = false; //遗嘱保留 

        public bool HasUserName => !string.IsNullOrEmpty(UserName);

        public bool HasPassword => !string.IsNullOrEmpty(Password);

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int KeepAliveSecond { get; set; } = 60;
    }
}
