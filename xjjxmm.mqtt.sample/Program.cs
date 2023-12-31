// See https://aka.ms/new-console-template for more information

//https://mcxiaoke.gitbooks.io/mqtt-cn/content/mqtt/02-ControlPacketFormat.html
//https://github.com/mcxiaoke/mqtt
//https://mqtt.p2hp.com/mqtt-5-0
//https://mcxiaoke.gitbooks.io/mqtt-cn/content/
using System.Net;
using System.Net.Sockets;
using xjjxmm.mqtt.sample.Client;


//await Client_Connection_Samples.Connect_Client();
//await Client_Publish_Samples.PublishQos2();
await Client_Subscribe_Samples.SubscribeQos0();


Console.WriteLine("Hello, World!");
Console.Read();