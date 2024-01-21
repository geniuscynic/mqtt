using mqtt.server.Options;
using xjjxmm.mqtt.Constant;

namespace xjjxmm.mqtt.MqttPacket;

internal class PingReqPacketFactory : IPacketFactory
{
    private readonly PingReqPacket packet;
    public PingReqPacketFactory(PingReqOption option)
    {
     
        
    }
    
    public PingReqPacketFactory(ReceivedPacket received)
    {
      
    }
    
    public PingReqPacketFactory(PingReqPacket option)
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