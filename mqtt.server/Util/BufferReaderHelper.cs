using System.Net.Sockets;
using System.Text;

namespace mqtt.server.Util;

//https://zhuanlan.zhihu.com/p/653496155
internal class BufferReaderHelper(ArraySegment<byte> bytes)
{
    private int pos = 0;

    public bool HasNext()
    {
        return pos < bytes.Count;
    }
    
    public byte Next()
    {
        return Next(1)[0];
    }
    
    public ArraySegment<byte> Next(int size)
    {
        var tmp = bytes[pos..(pos + size)];
        pos += size;
        return tmp;
    }
    
    public string NextStr(int size)
    {
        var tmp = Next(size);
        return Encoding.UTF8.GetString(tmp.ToArray(), 0, tmp.Count);        
    }
    
    public int NextTwoByteInt()
    {
        var tmp = Next(2);
        var msb = tmp[0];
        var lsb = tmp[1];
        return (msb << 8) | lsb;
    }
}
