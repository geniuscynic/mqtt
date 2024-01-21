using mqtt.server.Options;
using xjjxmm.mqtt.Options;

namespace xjjxmm.mqtt.MqttPacket;

internal class SubAckPacketFactory : IPacketFactory
{
    private readonly SubAckPacket packet;
    public SubAckPacketFactory(SubAckOption option, ushort packetIdentifier)
    {
        packet = new SubAckPacket()
        {
            PacketIdentifier = packetIdentifier,
            ReasonCodes = option.ReasonCodes
        };

    }
    
    public SubAckPacketFactory(ReceivedPacket received)
    {
        var option = new SubAckPacket();
        var readerHelper = received.GetPacketHelper();
        var packetIdentifier = readerHelper.NextTwoByteInt();
        option.PacketIdentifier = packetIdentifier;
        while (readerHelper.HasNext()) option.ReasonCodes.Add(readerHelper.Next());
    }
    
    public SubAckPacketFactory(SubAckPacket option)
    {
        this.packet = option;
    }
    
    public MqttPacket GetPacket()
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