using mqtt.server.Options;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Command;

internal interface ICommand
{
   
    IOption Decode(ReceivedPacket data);
    
    TaskCompletionSource<ReceivedPacket> Result { get; }
    
    public byte? AcceptCommand { get; }
}

internal interface ISendCommand : ICommand
{
    ArraySegment<byte> Encode();

}