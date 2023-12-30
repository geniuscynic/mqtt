using mqtt.server.Options;

namespace xjjxmm.mqtt.Options
{
    public record PubRecOption : IOption
    {
        public int PacketIdentifier { get; set; }
    }
}
