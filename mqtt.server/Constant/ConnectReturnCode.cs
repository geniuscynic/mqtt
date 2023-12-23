namespace mqtt.server.Constant;

public class ConnectReturnCode
{
    public const byte Accepted = 0x00;
    public const  byte RefusedUnacceptableProtocolVersion = 0x01;
    public const byte RefusedIdentifierRejected = 0x02;
    public const byte RefusedServerUnavailable = 0x03;
    public const byte RefusedBadUsernameOrPassword = 0x04;
    public const byte RefusedNotAuthorized = 0x05;
}