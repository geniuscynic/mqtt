using mqtt.server.Options;
using xjjxmm.mqtt.Command;
using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Channel;

internal abstract class BaseCommand : ICommand
{
    public abstract IOption Decode(ReceivedPacket data);

    public TaskCompletionSource<ReceivedPacket> Result { get; } = new();
    
    public abstract ArraySegment<byte> Encode();

   
    public abstract byte? AcceptCommand { get; }
    
   
}