using mqtt.server;
using mqtt.server.Constant;
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

    //private int publicIdentifier = 1;
    private HashSet<int> packetIdentifierHashSet = new();
    private PacketIdentifierProvider _packetIdentifierProvider = new ();
    
    public async Task Connect(ConnectOption option)
    {
        _mqttChannel.ConnAckAction = ConnAckAction;
        _mqttChannel.PingRespAction = PingRespAction;
        
        await _mqttChannel.SendConnect(option);

        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                await _mqttChannel.PingReq(new PingReqOption());
                await Task.Delay(option.KeepAliveSecond * 1000);
            }
        },  TaskCreationOptions.LongRunning);
    }
    
   
    public async Task Publish(PublishOption option)
    {
        if (option.QoS == Qos.AtLeastOnce)
        {
            _packetIdentifierProvider.Next();
            while (packetIdentifierHashSet.Contains(_packetIdentifierProvider.Current))
            {
                _packetIdentifierProvider.Next();
            }
            packetIdentifierHashSet.Add(_packetIdentifierProvider.Current);
            option.PacketIdentifier = _packetIdentifierProvider.Current;            
        }
        
        await _mqttChannel.SendPublish(option);

        if (option.QoS == Qos.AtLeastOnce)
        {
            await Task.Delay(1000);
            // 重发
            while (packetIdentifierHashSet.Contains(option.PacketIdentifier))
            {
                await _mqttChannel.SendPublish(option);
                await Task.Delay(1000);
            }
        }

        _mqttChannel.PubAckAction = ackOption =>
        {
            packetIdentifierHashSet.Remove(ackOption.PacketIdentifier);
        };
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