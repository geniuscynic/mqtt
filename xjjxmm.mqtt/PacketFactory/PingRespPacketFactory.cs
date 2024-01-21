using mqtt.server.Options;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class PingRespPacketFactory : IPacketFactory
{
    private readonly PingRespPacket packet;
    public PingRespPacketFactory(PingRespOption option)
    {
     
        
    }
    
    public PingRespPacketFactory(ReceivedPacket received)
    {
      
    }
    
    public PingRespPacketFactory(PingRespPacket option)
    {
        this.packet = option;
    }
    
    public MqttPacket GetPacket()
    {
        return packet;
    }

    private List<byte> Data { get; } = new List<byte>();
    
    protected void PushHeaders()
    {
        
    }

    protected void PushRemainingLength()
    {
        
    }

    protected void PushVariableHeader()
    {
    }

    protected void PushPayload()
    {
    }

    
    public ArraySegment<byte> Encode()
    {
        PushHeaders();
        PushRemainingLength();
        PushVariableHeader();
        PushPayload();
        return Data.ToArray();
    }

    public IOption GetOption()
    {
        return new PingRespOption();
    }
}