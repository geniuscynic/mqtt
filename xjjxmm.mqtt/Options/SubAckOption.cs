using mqtt.server.Constant;

namespace mqtt.server.Options
{
    public record SubAckOption : IOption
    {
        public List<byte> ReasonCodes { get; set; } = new List<byte>();
    }
}
