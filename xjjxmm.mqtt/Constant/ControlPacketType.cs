namespace xjjxmm.mqtt.Constant;


public enum ControlPacketType
{
    // The connection request is sent by the server to the client to
    // request a connection to the server.
    None = 0x00,

    // The connection request is sent by the server to the client to
    // request a connection to the server.
    Connect = 0x01,

    // The connection request is sent by the client to the server to
    // request a connection to the server.
    ConnAck = 0x02,

    // The publish message is sent by the client to the server to publish
    // a message to the server.
    Publish = 0x03,

    // The publish message is sent by the server to the client to publish
    // a message to the server.
   PubAck = 0x04,

    // The publish message is sent by the client to the server to publish
    // a message to the server.
    PubRec = 0x05,

    // The publish message is sent by the server to the client to publish
    // a message to the server.
    PubRel = 0x06,

    // The publish message is sent by the client to the server to publish
    // a message to the server.
    PubComp = 0x07,
            
    // The subscribe message is sent by the client to the server to
    // request a subscription to the server.
    Subscribe = 0x08,

    // The subscribe message is sent by the server to the client to
    // request a subscription to the server.
    SubAck = 0x09,

    // The unsubscribe message is sent by the client to the server to
    // request a unsubscription from the server.
    UnSubscribe = 0x0A,

    // The unsubscribe message is sent by the server to the client to
    // request a unsubscription from the server.
    UnSubAck = 0x0B,  

    // The ping request is sent by the client to the server to keep the
    // connection alive.
    PingReq = 0x0C,

    // The ping response is sent by the server to the client to keep
    // the connection alive.
    PingResp = 0x0D,

    // The disconnect message is sent by the client to the server to
    // terminate the connection.
    Disconnect = 0x0E,

    //mq5
    Auth = 0x0F,
}