using xjjxmm.mqtt.Util;

namespace xjjxmm.mqtt.Packet;

internal record struct ReceivedPacket
{
    public byte Header { get; }
    public int RemainingLength { get; private set; }
    
      
        private BufferReaderHelper _readerHelper;

        public ReceivedPacket(byte header, byte remainingLength, ArraySegment<byte> body)
        {
            Header = header;
            _readerHelper = new BufferReaderHelper(body);
            RemainingLength = remainingLength;
            ComputeRemainingLength();
        }
        
        public void Append(ArraySegment<byte> content)
        {
            _readerHelper.Append(content);
        }
        
    public BufferReaderHelper GetReaderHelper()
    {
        return _readerHelper;
    }
    
    /// <summary>
    /// 获取一个长度数据
    /// </summary>
    /// <returns></returns>
    private  void ComputeRemainingLength()
    {
        var i = 0;
        var value = RemainingLength;
        var multiplier = 1;

        int encodedByte = RemainingLength;
        while ((encodedByte & 128) != 0)
        {
            value += (_readerHelper.Next() & 127) * multiplier;
            value++;
            
            if (multiplier > 128 * 128 * 128)
                throw new Exception("字段太长，超出限制了");


            multiplier *= 128;

            encodedByte = _readerHelper.Next();

            i++;
        } 

        _readerHelper.Prev();
        RemainingLength = value;
    }
}