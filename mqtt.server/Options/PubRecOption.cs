using mqtt.server.Constant;

namespace mqtt.server.Options
{
    public record PubRecOption : IOption
    {
        public int PacketIdentifier { get; set; }
    }
}
