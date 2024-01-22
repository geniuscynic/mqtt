namespace xjjxmm.mqtt.Packet;

internal class SubAckPacket : IdentifierPacket
{
   
        
    public List<byte> ReasonCodes { get; set; } = new List<byte>();
    
   

  
}