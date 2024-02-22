using System.Net;
using System.Net.Sockets;
using xjjxmm.mqtt.Net;

namespace CNgork.Server;

public class Ports
{
    public int InPort { get; set; }
    public int OutPort { get; set; }
}

public class NgorkServer
{
    public async Task Start3()
    {
        var server = new SocketProxy();
        server.Bind(new IPEndPoint(IPAddress.Any, 9001));
        server.Listen(100);
        
        Console.WriteLine("接受代理连接在端口："+ 9001);
        
        while (true)
        {
            var client = await server.Accept();
            var size = await client.Receive(_buffer);
            if (size > 0)
            {
                Console.WriteLine("接受到数据："+ size);
            }
        }
        
    }
    
    public async Task Start()
    {
        var ports = new List<Ports>()
        {
            new Ports()
            {
                InPort = 9001,
                OutPort = 9000
            }
        };

        foreach (var port in ports)
        {
            Task.Run(async () =>
            {
                var outServer = await Listen(port.OutPort);
                Console.WriteLine("接受客户端连接在端口："+ port.OutPort);
                
                //var inServer = await Listen(port.InPort);
                //Console.WriteLine("接受代理连接在端口："+ port.InPort);
                var server = new SocketProxy();
                server.Bind(new IPEndPoint(IPAddress.Any, 9001));
                server.Listen(100);
                while (true)
                {
                    var inServer = await server.Accept();
                    Console.WriteLine("接受代理连接在端口："+ port.InPort);
                    Receive(inServer, outServer);
                    Receive(outServer, inServer);
                }
                
               
            });
        }

        while (true)
        {
            await Task.Delay(100000);
        }
    }

    private ArraySegment<byte> _buffer = new(new byte[1024000]);

    private async Task Receive(SocketProxy inServer, SocketProxy outServer)
    {
        while (true)
        {
            var size = await inServer.Receive(_buffer);
            if (size > 0)
            {
                await outServer.Send(_buffer[..size], default);
            }
        }
    }

  
    private async Task<SocketProxy> Listen(int port)
    {
        var server = new SocketProxy();
        server.Bind(new IPEndPoint(IPAddress.Any, port));
        server.Listen(100);
        var client = await server.Accept();
        return client;
    }
    

}