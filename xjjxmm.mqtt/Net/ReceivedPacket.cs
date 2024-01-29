using mqtt.client.test;
using mqtt.server.Constant;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.Net;
using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.MqttPacket;

internal class ReceivedPacket(SocketProxy socketProxy)
{
    private PacketHelper _packetHelper;
    private const int BUFFER_SIZE = 2;
    private ArraySegment<byte> _buffer = new (new byte[BUFFER_SIZE]);
    
    public int TotalLength = 0;
    public byte Header { get; private set; }
    public int RemainingLength { get; private set; }

    public PacketType GetPacketType()
    {
        var header = (Header & 0xF0) >> 4;
        return Enum.Parse<PacketType>(header.ToString());
    }

    public async Task<int> Receive()
    {
        var size = await socketProxy.Receive(_buffer); 
        if (size > 0)
        {
            Header = _buffer[0];
            if(Header == 0)
            {
                return size;
            }

            RemainingLength = _buffer[1];
            
            var body = await socketProxy.Receive(RemainingLength);
            _packetHelper = new PacketHelper(body);
            
            var remainingLength = ComputeRemainingLength();
            
            if (remainingLength > RemainingLength)
            {
                body = await socketProxy.Receive(TotalLength - RemainingLength - 1);
                RemainingLength = remainingLength;
                Append(body);
            }
        }

        return size;
    }
    
    private void Append(ArraySegment<byte> content)
    {
        _packetHelper.Append(content);
    }
    
    /// <summary>
    ///     获取一个长度数据
    /// </summary>
    /// <returns></returns>
    private int ComputeRemainingLength()
    {
        var i = 0;
        var value = RemainingLength & 127;
        var multiplier = 128;

        var encodedByte = RemainingLength;
        while ((encodedByte & 128) != 0)
        {
            var next = _packetHelper.Next();
            value += (next & 127) * multiplier;
            //value++;

            if (multiplier > 128 * 128 * 128)
                throw new Exception("字段太长，超出限制了");

            multiplier *= 128;

            encodedByte = next;

            i++;
        }

        // _readerHelper.Prev();
        // RemainingLength = value;
        TotalLength = value + i + 1;
        return value;
    }

    public PacketHelper GetPacketHelper()
    {
        return _packetHelper;
    }

    public void Dump()
    {
        _buffer.Dump("dump");
        _packetHelper.GetAll().Dump("dump");
    }
}