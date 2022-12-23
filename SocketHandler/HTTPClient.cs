using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using NetworkSocket.ProtocalHandler;
using System;

namespace NetworkSocket.SocketHandler
{
    public class HTTPClient
    {
        private const int BUFFER_SIZE = 1024;
        private TcpClient _tcpClient;
        private int _position;
        private CancellationTokenSource _cancelSource;
        
        public int ResquestCounter { get; private set; }
        public int Position { get => _position;}
        public bool isConnected => _tcpClient.Connected;
        public CancellationToken cancellation => _cancelSource.Token;
        public string Address { get; private set; }


        public HTTPClient(TcpClient client, int position)
        {
            _cancelSource= new CancellationTokenSource();
            _tcpClient = client;
            _position = position;
            ResquestCounter= 0;
            IPEndPoint? point = _tcpClient.Client.RemoteEndPoint as IPEndPoint;
            if (point != null) Address = point.Address.ToString() + ":" + point.Port;
            else Address = "";
        }
        public NetworkStream GetStream()
        {
            return new NetworkStream(_tcpClient.Client);
        }

        public void Close()
        {
            _cancelSource.Cancel();
            _tcpClient.Close();
        }

        public void Send(Response res)
        {
            NetworkStream stream = GetStream();
            stream.Write(res.getFullByte());
            if (res.Data != null) stream.Flush();
        }

        public Request? read()
        {
            try
            {
                NetworkStream stream = GetStream();
                MemoryStream memoryStream = new MemoryStream();
                var buffer = new byte[BUFFER_SIZE];
                int recieve_size;
                do
                {
                    recieve_size = stream.Read(buffer, 0, buffer.Length);
                    if (recieve_size == 0)
                    {
                        return null;
                    }
                    memoryStream.Write(buffer, 0, recieve_size);
                } while (stream.DataAvailable);

                ResquestCounter++;
                return new Request(memoryStream.ToArray());
            }
            catch (Exception e)
            {
                if (!cancellation.IsCancellationRequested)
                {
                    ExceptionHandler.ExceptionResponser.Response(e);
                }
                return null;
            }
        }
    }
}