namespace xjjxmm.mqtt.Options
{
    public record ConnAckOption : IOption
    {
        public bool IsSessionPresent { get; set; }
        
        public byte ReasonCode { get; set; }

        public override string ToString()
        {
            return $"ConnAck: [ReasonCode={ReasonCode}] [IsSessionPresent={IsSessionPresent}]";
        }
    }
}
