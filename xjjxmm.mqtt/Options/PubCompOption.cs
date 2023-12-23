using mqtt.server.Constant;

namespace mqtt.server.Options
{
    public record PubCompOption : IOption
    {
        public int PacketIdentifier { get; set; }
    }
}
