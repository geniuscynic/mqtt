using System.Net;
using System.Net.Sockets;
using System.Text;
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
                var outServer = Listen(port.OutPort);
                var outClient = await outServer.Accept();
                Console.WriteLine("接受客户端连接在端口："+ port.OutPort);
                
                //var inServer = await Listen(port.InPort);
                //Console.WriteLine("接受代理连接在端口："+ port.InPort);
                var inServer = Listen(port.InPort);
             
                while (true)
                {
                    var inClient = await inServer.Accept();
                    Console.WriteLine("接受代理连接在端口："+ port.InPort);
                    Receive(inClient, outClient);
                   // Receive(outClient, inClient);
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
                
                while (true)
                {
                    var size2 = await outServer.Receive(_buffer);
                    if (size2 > 0)
                    {
                        await inServer.Send(_buffer[..size2], default);
                    }
                }
            }
        }
    }

  
    private SocketProxy Listen(int port)
    {
        var server = new SocketProxy();
        server.Bind(new IPEndPoint(IPAddress.Any, port));
        server.Listen(100);
        
        return server;
    }
    
    private const int BufferSize = 4096;
    private async Task HandleClient(Socket client)
    {
        using (var clientStream = new NetworkStream(client))
        {
            byte[] buffer = new byte[BufferSize];
            int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);

            string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            string[] requestLines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);
            string[] firstLineParts = requestLines[0].Split(' ');
            string method = firstLineParts[0];
            string url = firstLineParts[1];

            Uri uri = new Uri(url);
            string host = uri.Host;
            int port = uri.Port == -1 ? 80 : uri.Port;

            using (var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                await serverSocket.ConnectAsync(host, port);

                await serverSocket.SendAsync(Encoding.ASCII.GetBytes(request), SocketFlags.None);

                byte[] serverBuffer = new byte[BufferSize];
                int serverBytesRead = await serverSocket.ReceiveAsync(serverBuffer, 0, serverBuffer.Length, SocketFlags.None);

                await clientStream.WriteAsync(serverBuffer, 0, serverBytesRead);
            }
        }

        client.Close();
    }
}
}