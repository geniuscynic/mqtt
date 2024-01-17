using mqtt.server.Options;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Net;

namespace xjjxmm.mqtt.Command;

internal abstract class BaseCommand(MqttChannel3 mqttChannel)
{
    public abstract Task Send(IOption option);
    public abstract Task<IOption> GetResult();
    
    public TaskCompletionSource<ReceivedPacket> Result { get; } = new (TaskCreationOptions.RunContinuationsAsynchronously);
    
}

