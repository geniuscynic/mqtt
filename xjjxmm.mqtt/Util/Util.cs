namespace mqtt.client.test
{
    public static class UtilHelpers

    {
    public static byte[] ToBytes(this string str)
    {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    public static string ToStrings(this List<byte> bytes)
    {
        return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
    }

    public static string ToStrings(this Span<byte> str)
    {
        return System.Text.Encoding.UTF8.GetString(str);
    }

    public static void Dump(this List<byte> list)
    {
        foreach (var i in list)
        {
            Console.Write(i);
            Console.Write(" ");
        }

        Console.WriteLine(" ");
    }

    public static void Dump(this ArraySegment<byte> list, string name)
    {
        Console.Write(name + " ");
        foreach (var i in list)
        {
            Console.Write(i);
            Console.Write(" ");
        }

        Console.WriteLine(" ");
    }

    public static void Dump(this string msg)
    {
       
        Console.WriteLine(msg);
    }
    
    public static IEnumerable<int> ComputeRemainingLength(int len)
    {
        do
        {
            var encodeByte = len % 128;
            len = len / 128;
            if (len > 0)
            {
                encodeByte = encodeByte | 128;
            }

            yield return encodeByte;
        } while (len > 0);
    }

    /// <summary>
    /// 获取一个长度数据
    /// </summary>
    /// <returns></returns>
    public static int ComputeRemainingLength(Span<byte> bytes)
    {
        var i = 0;
        var value = 0;
        var multiplier = 1;

        int encodedByte;
        do
        {
            value += (bytes[i] & 127) * multiplier;

            if (multiplier > 128 * 128 * 128)
                throw new Exception("字段太长，超出限制了");


            multiplier *= 128;

            encodedByte = bytes[i];

            i++;
        } while ((encodedByte & 128) != 0);


        return value;
    }


    public static byte RandomByte()
    {
        var random = new Random();
        var res = random.Next(0, 256);
        return (byte)res;
    }

    }
}