using mqtt.server.Constant;

namespace mqtt.server.Options
{
    public record PubRelOption : IOption
    {
        public int PacketIdentifier { get; set; }
    }
}
