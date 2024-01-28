﻿using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;
using xjjxmm.mqtt.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Adapt;

internal class PubAckPacketAdapt : IAdaptFactory
{
    private readonly PubAckPacket packet;
    public PubAckPacketAdapt(PubAckOption option, ushort packetIdentifier)
    {
        packet = new PubAckPacket
        {
            PacketIdentifier = packetIdentifier
        };

    }
    
    public PubAckPacketAdapt(ReceivedPacket received)
    {
        var helper = received.GetPacketHelper();
        var packetIdentifier = helper.NextTwoByteInt();

        packet = new PubAckPacket
        {
            PacketIdentifier = packetIdentifier
        };
    }
    
    public PubAckPacketAdapt(PubAckPacket option)
    {
        this.packet = option;
    }
    
    public Packet.MqttPacket GetPacket()
    {
        return packet;
    }

    private List<byte> Data { get; } = new List<byte>();
    
   
    protected  void PushHeaders()
    {
        byte header = (byte)PacketType.PubAck << 4;

        Data.Add(header);
    }

    protected  void PushRemainingLength()
    {
        Data.Add(0x02);
    }

    protected  void PushVariableHeader()
    {
        var msb = (byte)(packet.PacketIdentifier >> 8);
        var lsb = (byte)(packet.PacketIdentifier & 255);
        Data.Add(msb);
        Data.Add(lsb);
    }

    protected  void PushPayload()
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
        return new PubAckOption
        {
           // PacketIdentifier = packet.PacketIdentifier
        };
    }
}