﻿using mqtt.server.Options;

namespace xjjxmm.mqtt.Options
{
    public record PubAckOption : IOption
    {
        public ushort PacketIdentifier { get; set; }
    }
}
