using mqtt.server;
using mqtt.server.Options;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt;

public class MqttClient : IDisposable
{
    private readonly MqttChannel _mqttChannel = new();

    public Action<ConnAckOption>? ConnAckAction { get; set; }

    public Action<SubAckOption>? SubAckAction{ get; set; }

    public Action<PingRespOption>? PingRespAction{ get; set; }

    public Action<PubAckOption>? PubAckAction{ get; set; }
    
    public Action<PubRecOption>? PubRecAction{ get; set; }
    
    public Action<PubRelOption>? PubRelAction{ get; set; }
    
    public Action<PubCompOption>? PubCompAction{ get; set; }
    
    public Action<UnSubAckOption>? UnSubAckAction{ get; set; }

    //创建与远程主机的连接

    public async Task Connect(ConnectOption option)
    {
        await _mqttChannel.Connect(option);
        
        while (true)
        {
            await _mqttChannel.PingReq(new PingReqOption());
            await Task.Delay(option.KeepAliveSecond);
        }
    }
    
   
    public async Task Publish(PublishOption option)
    {
        await _mqttChannel.Publish(option);
    }
    
    public async Task Subscribe(SubscribeOption option)
    {
        await _mqttChannel.Subscribe(option);
    }
    
    public void Dispose()
    {
        //socket.Close();
    }

}