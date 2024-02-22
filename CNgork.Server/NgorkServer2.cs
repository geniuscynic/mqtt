using System.Net;
using System.Net.Sockets;
using xjjxmm.mqtt.Net;

namespace CNgork.Server;



public class NgorkServer2
{
    
    
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
                
                var inServer = await Listen(port.InPort);
                Console.WriteLine("接受代理连接在端口："+ port.InPort);
                
  
                    Receive(inServer, outServer);
                    Receive(outServer, inServer);
               
                
               
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