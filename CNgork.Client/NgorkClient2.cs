using System.Net;
using System.Net.Sockets;
using xjjxmm.mqtt.Net;

namespace CNgork.Client;



public class NgorkClient2
{
    public async Task Start()
    {
        var proxyConfigs = new List<ProxyConfig>
        {
            new ProxyConfig()
            {
                ServerIp = "127.0.0.1",
                ServerPort = 9000,

                LocalIp = "106.14.75.229",
                LocalPort = 22
            }
        };

        foreach (var proxyConfig in proxyConfigs)
        {
            Task.Run(async () =>
            {
                var main = await Connect(proxyConfig.ServerIp, proxyConfig.ServerPort);
                Console.WriteLine("连接到服务端");
                
                 var tcp1 = await Connect(proxyConfig.LocalIp, proxyConfig.LocalPort);
                Console.WriteLine("连接到本地");

                 Receive(main, tcp1);
                 Receive(tcp1, main);
            });
        }


        while (true)
        {
            await Task.Delay(100000);
        }
    }

    private ArraySegment<byte> _buffer = new(new byte[1024000]);

    private async Task Receive(SocketProxy mainServer, ProxyConfig proxyConfig)
    {
        while (true)
        {
            var size = await mainServer.Receive(_buffer);
            if (size > 0)
            {
                 var tcpClient = await Connect(proxyConfig.LocalIp, proxyConfig.LocalPort);
                 Receive(tcpClient, mainServer);
                 
                 Console.WriteLine("连接到本地");
                 await tcpClient.Send(_buffer[..size], default);
                 
                 
            }
        }
    }

    
    public async Task Receive(SocketProxy mainServer, SocketProxy tcpClient)
    {
        while (true)
        {
            var size = await mainServer.Receive(_buffer);
            if (size > 0)
            {
                await tcpClient.Send(_buffer[..size], default);
            }
        }
    }


    private async Task<SocketProxy> Connect(string ip, int port)
    {
        var client = new SocketProxy();
        await client.Connect(ip, port);
        
        return client;
    }
}