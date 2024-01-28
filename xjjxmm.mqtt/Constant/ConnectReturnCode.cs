namespace xjjxmm.mqtt.Constant;

public enum ConnectReturnCode
{ 
   Accepted = 0x00,
   RefusedUnacceptableProtocolVersion = 0x01,
   RefusedIdentifierRejected = 0x02,
   RefusedServerUnavailable = 0x03,
   RefusedBadUsernameOrPassword = 0x04,
   RefusedNotAuthorized = 0x05
}