using mqtt.server.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Command;

internal interface ICommand
{
    ArraySegment<byte> Encode();

    IOption Decode(ReceivedPacket data);
    
    TaskCompletionSource<ReceivedPacket> Result { get; }
    
    public byte? AcceptCommand { get; }
}