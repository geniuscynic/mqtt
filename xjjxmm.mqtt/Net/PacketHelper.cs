using System.Text;
using mqtt.server.Options;

namespace xjjxmm.mqtt.Net;

public class PacketHelper(ArraySegment<byte> bytes)
{
    private int pos = 0;

    public bool HasNext()
    {
        return pos < bytes.Count;
    }
   
    public void Prev()
    {
        pos--;
        if (pos < 0)
        {
            pos = 0;
        }
    }
    
    public byte Next()
    {
        return Next(1)[0];
    }
    
    public ArraySegment<byte> NextAll()
    {
        var size = bytes.Count;
        return Next(size);
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
    
    public ushort NextTwoByteInt()
    {
        var tmp = Next(2);
        var msb = tmp[0];
        var lsb = tmp[1];
        return (ushort)((msb << 8) | lsb);
    }

    public void Append(ArraySegment<byte> body)
    {
        bytes = bytes.Concat(body)
            .ToArray();
    }

    public ArraySegment<byte> Get(int start, int size)
    {
        return bytes[start..(start + size)];
    }

    public ArraySegment<byte> GetAll()
    {
        return bytes;
    }
}