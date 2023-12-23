using mqtt.server.Util;

namespace mqtt.server;

internal record struct ReceivedPacket(byte Header, byte RemainingLength, ArraySegment<byte> Body)
{
    public BufferReaderHelper GetReaderHelper()
    {
        return new BufferReaderHelper(Body);
    }
}