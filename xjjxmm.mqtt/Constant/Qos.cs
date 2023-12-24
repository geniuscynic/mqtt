namespace mqtt.server.Constant;

public class Qos
{
    // QoS 0 is the default value
    public const byte AtMostOnce = 0x00;

    // QoS 1 is the same as QoS 0
    public const byte AtLeastOnce = 0x01;

    // QoS 2 is the same as QoS 1
    public const byte ExactlyOnce = 0x02;
}