using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class SubAckPacket : MqttPacket
{
    public ushort PacketIdentifier { get; set; }
        
    public List<byte> ReasonCodes { get; set; } = new List<byte>();
    
   

  
}