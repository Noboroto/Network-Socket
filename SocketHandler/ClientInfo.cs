using System.Net.Sockets;
using System.Net;

namespace NetworkSocket.SocketHandler
{
    public class ClientInfo
    {
        private TcpClient _tcpClient;
        private int _position;
        
        public int Position { get => _position;}
        public bool isConnected => _tcpClient.Connected;

        public ClientInfo(TcpClient client, int position)
        {
            _tcpClient = client;
            _position = position; 
        }
        public NetworkStream GetStream()
        {
            return new NetworkStream(_tcpClient.Client);
        }

        public void Close()
        {
            _tcpClient.Close();
        }

        public string Address 
        {
            get
            {
                IPEndPoint? point = _tcpClient.Client.RemoteEndPoint as IPEndPoint;
                if (point != null) return point.Address.ToString() + ":"+ point.Port;
                return "";
            }
        }
    }
}