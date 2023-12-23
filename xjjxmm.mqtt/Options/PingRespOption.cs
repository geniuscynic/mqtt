using mqtt.server.Options;

namespace xjjxmm.mqtt.Options;

public record PingRespOption : IOption
{
    public override string ToString()
    {
        return "PingResp";
    }
}