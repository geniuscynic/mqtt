using System.Net;
using System.Net.Sockets;

namespace xjjxmm.mqtt.Net
{
    //https://zhuanlan.zhihu.com/p/653496155
    internal class SocketProxy
    {
        private const int ChunkSize = 3;
        private readonly Socket _socket;

        public SocketProxy()
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }
        
        public SocketProxy(Socket socket)
        {
            _socket = socket;
        }

        public EndPoint? LocalEndPoint => _socket.LocalEndPoint;
        /// <summary>
        /// 绑定socket到本地地址和端口，通常由服务端调用
        /// </summary>
        /// <param name="localEndPoint"></param>
        public void Bind(EndPoint localEndPoint)
        {
            _socket.Bind(localEndPoint);
        }
       
        /// <summary>
        /// TCP专用，开启监听模式
        /// </summary>
        /// <param name="connectionBacklog"></param>
        public void Listen(int connectionBacklog)
        {
            _socket.Listen(connectionBacklog);
        }
        
        /// <summary>
        /// TCP专用，服务器等待客户端连接，一般是阻塞态
        /// </summary>
        /// <returns></returns>
        public async Task<SocketProxy> Accept()
        {
            return new  SocketProxy(await _socket.AcceptAsync());
        }
       
        /// <summary>
        /// TCP专用，客户端主动连接服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="cancellationToken"></param>
        public async Task Connect(string host, int port)
        {
            await _socket.ConnectAsync(host, port);
        }
        
        /// <summary>
        /// TCP专用，客户端主动连接服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="cancellationToken"></param>
        public async Task Connect(string host, int port, CancellationToken cancellationToken)
        {
            await _socket.ConnectAsync(host, port, cancellationToken);
        }
        
        /// <summary>
        /// TCP专用，发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public async Task<int> Send(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            return await _socket.SendAsync(buffer, SocketFlags.None, cancellationToken);
        }
        
        /// <summary>
        /// TCP专用，发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public async Task<int> Send(ArraySegment<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken)
        {
            return await _socket.SendAsync(buffer, socketFlags, cancellationToken);
        }
        
        /// <summary>
        /// TCP专用，接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="socketFlags"></param>
        /// <returns></returns>
        public async Task<int> Receive(ArraySegment<byte> buffer)
        {
            return await _socket.ReceiveAsync(buffer);
        }

        /// <summary>
        /// TCP专用，接收数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<int> Receive(ArraySegment<byte> buffer,  int start, int length)
        {
            return await _socket.ReceiveAsync(buffer[start..(start+length)]);
        }
        
        public async Task<ArraySegment<byte>> Receive(int totalLength)
        {
            var buffer = new ArraySegment<byte>(new byte[totalLength]);

            int left = 0;
            int right =  Math.Min(ChunkSize, totalLength);

            while (right > left)
            {
                await _socket.ReceiveAsync(buffer[left..right]);
                left = right;
                var remain = totalLength - right;
                right = left + Math.Min(ChunkSize, remain);                
            }

            return buffer;
        }

        /// <summary>
        /// sendto()：UDP专用，发送数据到指定的IP地址和端口
        ///recvfrom()：UDP专用，接收数据，返回数据远端的IP地址和端口
        /// </summary>
        public void Close()
        {
            _socket.Close();
        }
    }
}
