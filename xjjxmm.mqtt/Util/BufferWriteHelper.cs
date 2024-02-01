using System.Text;
using mqtt.client.test;

namespace xjjxmm.mqtt.Util;

//https://zhuanlan.zhihu.com/p/653496155
internal class BufferWriteHelper
{
   private List<byte> _data  = new List<byte>();

   private byte _header;
   public void SetHeader(byte header)
   {
      _header = header;
   }
   
   public void AddPacketIdentifier(ushort packetIdentifier)
   {
      var msb = (byte)(packetIdentifier >> 8);
      var lsb = (byte)(packetIdentifier & 255);
      _data.Add(msb);
      _data.Add(lsb);
   }

   public void AddByte(byte value)
   {
      _data.Add(value);
   }

   public ArraySegment<byte> Build()
   {
      var res = new List<byte>();
      res.Add(_header);
      var len = _data.Count;
     
      foreach (var l in UtilHelpers.ComputeRemainingLength(len))
      {
         res.Add(Convert.ToByte(l));
      }
      
      res.AddRange(_data);

      return res.ToArray();
   }
}
