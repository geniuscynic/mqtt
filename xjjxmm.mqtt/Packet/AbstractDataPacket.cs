using xjjxmm.mqtt.Constant;

namespace xjjxmm.mqtt.Packet;



internal abstract class AbstractDataPacket
{
   
    protected List<byte> Data { get; set; } = new();

    public ArraySegment<byte> Encode()
    {
        PushHeaders();
        PushRemainingLength();
        PushVariableHeader();
        PushPayload();

        //Data.Dump();

        return Data.ToArray();
    }

    //public abstract IOption Decode();

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

    public abstract PacketType GetPacketType();
}