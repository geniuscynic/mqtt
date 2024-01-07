using xjjxmm.mqtt.Packet;

namespace xjjxmm.mqtt.Channel;

internal abstract class BaseCommand : ICommand
{
    public TaskCompletionSource<ReceivedPacket> Result { get; } = new();
    
    public abstract ArraySegment<byte> Encode();

   
    public abstract byte AcceptCommand { get; }
}