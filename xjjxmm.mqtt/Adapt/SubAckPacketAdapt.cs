using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class SubAckPacketAdapt : IAdaptFactory
{
    private readonly SubAckPacket packet;
    public SubAckPacketAdapt(SubAckOption option, ushort packetIdentifier)
    {
        packet = new SubAckPacket()
        {
            PacketIdentifier = packetIdentifier,
            ReasonCodes = option.ReasonCodes
        };

    }
    
    public SubAckPacketAdapt(ReceivedPacket received)
    {
        var option = new SubAckPacket();
        var readerHelper = received.GetPacketHelper();
        var packetIdentifier = readerHelper.NextTwoByteInt();
        option.PacketIdentifier = packetIdentifier;
        while (readerHelper.HasNext()) option.ReasonCodes.Add(readerHelper.Next());

        this.packet = option;
    }
    
    public SubAckPacketAdapt(SubAckPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    private List<byte> Data { get; } = new List<byte>();
    
    protected void PushHeaders()
    {
        
    }

    protected void PushRemainingLength()
    {
        
    }

    protected void PushVariableHeader()
    {
    }

    protected void PushPayload()
    {
    }

    
    public ArraySegment<byte> Encode()
    {
        PushHeaders();
        PushRemainingLength();
        PushVariableHeader();
        PushPayload();
        return Data.ToArray();
    }

    public IOption GetOption()
    {
        return new  SubAckOption()
        {
            //PacketIdentifier = packet.PacketIdentifier,
            ReasonCodes = packet.ReasonCodes
        };
    }
}