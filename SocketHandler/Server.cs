using NetworkSocket.ExceptionHandler;
using NetworkSocket.ProtocalHandler;
using NetworkSocket.ProtocolHandler;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSocket.SocketHandler
{
    public class Server
    {
        private CancellationTokenSource _serverCancellation;
        private List<HTTPClient> _browserClients = new List<HTTPClient>();
        private TcpListener _serverListener;

        public List<HTTPClient> BrowserClients => new List<HTTPClient>(_browserClients);
        public int Port { get; private set; }
        public string SrcPath { get; set; }

        public Server(int inputPort = 8080, string srcPath = "./http/")
        {
            Port = inputPort;
            SrcPath = srcPath;
            _serverListener = new TcpListener(System.Net.IPAddress.Any, Port);
            _serverCancellation = new CancellationTokenSource();
        }

        ~Server()
        {
            Close();
        }

        public void Close()
        {
            _serverCancellation.Cancel();
            foreach (var client in _browserClients)
            {
                client.Close();
            }
            _serverListener.Stop();
        }

        public Task StartListenAsync()
        {
            return Task.Run(() => StartListen());
        }

        public void StartListen()
        {
            _serverListener.Start();
            TcpClient? client = new TcpClient();
            while (true)
            {
                if (_serverCancellation.IsCancellationRequested) return;
                if (_serverListener.Pending())
                {
                    client = _serverListener.AcceptTcpClient();

                    var pos = _browserClients.Count;
                    if (!client.Connected) continue;
                    _browserClients.Add(new HTTPClient(client, pos));

                    ListenFromClientAsync(pos);
                }
            }
        }

        private void ListenFromClientAsync(int pos)
        {
            try
            {
                Task.Run(() =>
                {
                    Request? req = null;
                    bool canContinue = true;
                    var client = _browserClients[pos];

                    Console.WriteLine($"Start listening client at {pos} - ip {client.Address}");

                    while (canContinue)
                    {
                        try
                        {
                            req = client.read();
                        }
                        catch (Exception e)
                        {
                            ExceptionResponser.Response(e);
                            canContinue = false;
                        }
                        if (req != null)
                        {
                            Response? res = RequestHandler.Handle(req, SrcPath);
                            if (res == null)
                            {
                                Console.WriteLine($"Client {pos} at {client.Address} - request {client.ResquestCounter} time" + ((client.ResquestCounter > 1) ? "s" : "") + $"\r\n{req}\nServer not support this Request!");
                                continue;
                            }
                            client.Send(res);
                            Console.WriteLine($"Client {pos} at {client.Address} - request {client.ResquestCounter} time" + ((client.ResquestCounter > 1) ? "s" : "") + $"\r\n{req}\n{res}\n");
                        }
                        else
                        {
                            Console.WriteLine($"Client {pos} at {client.Address} has disconnected...");
                            client.Close();
                            canContinue = false;
                        }
                    }
                }, _browserClients[pos].cancellation);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Client {pos} at {_browserClients[pos].Address} has been stopped...");
            }
        }
    }
}