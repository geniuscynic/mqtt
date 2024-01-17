using mqtt.server.Options;
using xjjxmm.mqtt.Constant;
using xjjxmm.mqtt.MqttPacket;

namespace xjjxmm.mqtt.Command;

internal interface ICommand
{
    Task Send(IOption option);
    Task<IOption> GetResult();
    Task SetResult(ReceivedPacket receivedPacket);
    Task<bool> IsHandel(ReceivedPacket receivedPacket);
    
    //PacketType PacketType { get; }
    //ArraySegment<byte> Encode();
}

internal interface IReceiveCommand : ICommand
{
    //public PacketType CommandPacketType { get; }
    IOption Decode(ReceivedPacket data);
    TaskCompletionSource<ReceivedPacket> Result { get; }
}