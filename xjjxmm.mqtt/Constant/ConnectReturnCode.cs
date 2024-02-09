namespace xjjxmm.mqtt.Constant;

public enum ConnectReturnCode
{ 
    //mq3
   Accepted = 0x00,
   RefusedUnacceptableProtocolVersion = 0x01,
   RefusedIdentifierRejected = 0x02,
   RefusedServerUnavailable = 0x03,
   RefusedBadUsernameOrPassword = 0x04,
   RefusedNotAuthorized = 0x05,

   //mq5
    Success = 0,
    UnspecifiedError = 128,
    MalformedPacket = 129,
    ProtocolError = 130,
    ImplementationSpecificError = 131,
    UnsupportedProtocolVersion = 132,
    ClientIdentifierNotValid = 133,
    BadUserNameOrPassword = 134,
    NotAuthorized = 135,
    ServerUnavailable = 136,
    ServerBusy = 137,
    Banned = 138,
    BadAuthenticationMethod = 140,
    TopicNameInvalid = 144,
    PacketTooLarge = 149,
    QuotaExceeded = 151,
    PayloadFormatInvalid = 153,
    RetainNotSupported = 154,
    QoSNotSupported = 155,
    UseAnotherServer = 156,
    ServerMoved = 157,
    ConnectionRateExceeded = 159
}