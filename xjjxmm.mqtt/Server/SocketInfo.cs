using xjjxmm.mqtt.Client;

namespace xjjxmm.mqtt.Server;

internal class SocketInfo
{
    public int Id { get; set; }

    public string ClientId { get; set; }

    //public Dispatcher Dispatcher { get; set; }
    public MqttClientChannel Channel { get; set; }

    public Dictionary<string, byte> SubscribeInfos { get; set; } = new();
    
    public DateTime LastLiveTime { get; set; }
}