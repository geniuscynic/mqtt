using mqtt.server.Constant;
using mqtt.server.Options;

namespace mqtt.server.Packet;

internal class PubRelPacket : AbstractDataPacket
{

    private readonly byte msb;
    private readonly byte lsb;

    public PubRelPacket()
    {
        
    }
    
    public PubRelPacket(int packetIdentifier)
    {
        msb = (byte)(packetIdentifier >> 8);
        lsb = (byte)(packetIdentifier & 255);
    }

    public PubRelPacket(PubRelOption option)
    {
        
    }

    protected override void PushHeaders()
    {
        var header = PacketType.PUBREL << 4;

        Data.Add(Convert.ToByte(header));
    }

    protected override void PushRemainingLength()
    {
        //剩余长度（Remaining Length）表示当前报文剩余部分的字节数，包括可变报头和负载的数据。剩余长度不包括用于编码剩余长度字段本身的字节数。也就是剩余长度 = 可变报头 + 有效载荷。
        Data.Add(0x02);
    }

    protected override void PushVariableHeader()
    {

        Data.Add(msb);
        Data.Add(lsb);

    }


    protected override void PushPayload()
    {


    }

    public override IOption Decode(ReceivedPacket buffer)
    {
        throw new NotImplementedException();
    }
}