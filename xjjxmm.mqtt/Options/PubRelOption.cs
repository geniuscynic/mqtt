using mqtt.server.Options;

namespace xjjxmm.mqtt.Options
{
    public record PubRelOption : IOption
    {
        public int PacketIdentifier { get; set; }
    }
}
