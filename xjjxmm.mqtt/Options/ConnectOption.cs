using mqtt.server.Constant;

namespace xjjxmm.mqtt.Options
{
    public class ConnectOption(string host, int port, string clientId) : IOption
    {
        public string Host { get; set; } = host;
        public int Port { get; set; } = port;
        
        public string ClientId { get; set; } = clientId;
        
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
