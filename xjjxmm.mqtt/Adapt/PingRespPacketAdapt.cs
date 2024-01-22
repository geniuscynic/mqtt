using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class PingRespPacketAdapt : IAdaptFactory
{
    private readonly PingRespPacket packet;
    public PingRespPacketAdapt(PingRespOption option)
    {
     
        
    }
    
    public PingRespPacketAdapt(ReceivedPacket received)
    {
      
    }
    
    public PingRespPacketAdapt(PingRespPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
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