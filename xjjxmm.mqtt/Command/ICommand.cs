using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Channel;

internal interface ICommand
{
    ArraySegment<byte> Encode();
    
    TaskCompletionSource<ReceivedPacket> Result { get; }
    
    public byte AcceptCommand { get; }
}