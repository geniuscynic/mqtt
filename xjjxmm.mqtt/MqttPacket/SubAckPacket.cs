using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class SubAckPacket : IdentifierPacket
{
   
        
    public List<byte> ReasonCodes { get; set; } = new List<byte>();
    
   

  
}