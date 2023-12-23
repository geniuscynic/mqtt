using mqtt.server.Constant;

namespace mqtt.server.Options
{
    public record ConnAckOption : IOption
    {
        public bool IsSessionPresent { get; set; }
        
        public byte ReasonCode { get; set; }
    }
}
