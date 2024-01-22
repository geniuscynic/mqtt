using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class PingReqPacketAdapt : IAdaptFactory
{
    private readonly PingReqPacket packet;
    public PingReqPacketAdapt(PingReqOption option)
    {
     
        
    }
    
    public PingReqPacketAdapt(ReceivedPacket received)
    {
      
    }
    
    public PingReqPacketAdapt(PingReqPacket option)
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
        byte header = (byte)PacketType.PingReq << 4;

        Data.Add(header);
    }

    protected void PushRemainingLength()
    {
        Data.Add(0x00);
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
        return new PingReqOption();
    }
}