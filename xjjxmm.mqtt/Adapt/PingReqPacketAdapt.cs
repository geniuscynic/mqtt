using mqtt.client.test;
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
    
    public ArraySegment<byte> Encode()
    {
        var bytes = new ArraySegment<byte>(new byte[]
        {
            (byte)PacketType.PingReq << 4,
            0x00
        });

        bytes.Dump("ping");
        return bytes;
    }

    public IOption GetOption()
    {
        return new PingReqOption();
    }
}