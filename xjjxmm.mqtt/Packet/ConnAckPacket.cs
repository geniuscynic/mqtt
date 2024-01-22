namespace xjjxmm.mqtt.Packet;

internal class ConnAckPacket : MqttPacket
{
    public bool IsSessionPresent { get; set; }
        
    public byte ReasonCode { get; set; }

    public override string ToString()
    {
        return $"ConnAck: [ReasonCode={ReasonCode}] [IsSessionPresent={IsSessionPresent}]";
    }
    
}