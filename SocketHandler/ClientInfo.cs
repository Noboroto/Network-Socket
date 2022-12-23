using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace NetworkSocket.SocketHandler
{
    public class ClientInfo
    {
        private TcpClient _tcpClient;
        private int _position;
        private CancellationTokenSource _cancelSource;
        
        public int ResquestCounter { get; set; }
        public int Position { get => _position;}
        public bool isConnected => _tcpClient.Connected;
        public CancellationToken cancellation => _cancelSource.Token;

        public ClientInfo(TcpClient client, int position)
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
            _tcpClient.Close();
            _cancelSource.Cancel();
        }

        public string Address { get; private set; }
    }
}