namespace xjjxmm.mqtt.Options
{
    public record SubAckOption : IOption
    {
       // public ushort PacketIdentifier { get; set; }
        
        public List<byte> ReasonCodes { get; set; } = new List<byte>();
    }
}
