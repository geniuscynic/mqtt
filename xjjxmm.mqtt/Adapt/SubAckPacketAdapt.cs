using mqtt.client.test;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;
using xjjxmm.mqtt.Util;

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
    
    public ArraySegment<byte> Encode()
    {
        var packetType = (byte)ControlPacketType.SubAck << 4;

        var writeHelper = new BufferWriteHelper();
        writeHelper.SetHeader((byte)packetType);
        writeHelper.AddPacketIdentifier(packet.PacketIdentifier);
        foreach (var packetReasonCode in packet.ReasonCodes)
        {
            writeHelper.AddByte(packetReasonCode);
        }

      //  writeHelper.Build().Dump("suback");
        return writeHelper.Build();
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