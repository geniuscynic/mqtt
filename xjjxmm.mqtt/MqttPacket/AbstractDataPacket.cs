using mqtt.client.test;
using mqtt.server.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal abstract class AbstractDataPacket<T> : IDataPacket<T> where T : IOption
{
    protected List<byte> Data { get; set; } = new();

    public ArraySegment<byte> Encode()
    {
        PushHeaders();
        PushRemainingLength();
        PushVariableHeader();
        PushPayload();

        Data.Dump();

        return Data.ToArray();
    }

    public abstract T Decode();

    /// <summary>
    ///     固定报头
    /// </summary>
    /// <returns></returns>
    protected abstract void PushHeaders();

    /// <summary>
    ///     剩余长度
    /// </summary>
    /// <returns></returns>
    protected abstract void PushRemainingLength();

    /// <summary>
    ///     可变报头
    /// </summary>
    /// <returns></returns>
    protected abstract void PushVariableHeader();

    /// <summary>
    ///     有效载荷
    /// </summary>
    /// <returns></returns>
    protected abstract void PushPayload();
}