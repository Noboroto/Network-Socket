using NetworkSocket.ExceptionHandler;
using NetworkSocket.ProtocalHandler;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket.SocketHandler;

public class Server
{
    private const int BUFFER_SIZE = 1024;

    private static List<ClientInfo> _browserClients = new List<ClientInfo>();
    private static TcpListener? _serverListener;
    private static readonly Task _initTask = new Task(() => {});
    private static Encoding _encoder = Encoding.UTF8;
    private static StringBuilder _strBuilder = new StringBuilder();
    
    public static void StartListen(int port)
    {
        if (_serverListener == null)
        {
            try
            {
                _initTask.Start();
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }
        _serverListener = new TcpListener(System.Net.IPAddress.Any, port);
        _serverListener.Start();
        TcpClient? client = new TcpClient();
        while (true)
        {
            if (_serverListener.Pending())
            {
                client = _serverListener.AcceptTcpClient();
                var pos = _browserClients.Count;
                if (!client.Connected) continue;
                _browserClients.Add(new ClientInfo(client, pos));
                Console.WriteLine(pos + 1);
                _strBuilder.Clear();
                _strBuilder.AppendLine((pos + 1).ToString());
                foreach (var c in _browserClients)
                {
                    _strBuilder.Append(c.isConnected + " ");
                }
                Console.WriteLine(_strBuilder.ToString());
                ListenFromClient(pos);
            }
        }
    }

    private static void ListenFromClient(int pos)
    {
        Task.Run(() =>
        {
            int counter = 0;
            string? information = null;
            bool canContinue = true;
            var client = _browserClients[pos];
            var Reader = client.GetStream();

            Console.WriteLine($"Start listening client at: {pos}");

            while (canContinue)
            {
                try
                {
                    Console.WriteLine($"Prepare listen {pos}");
                    information = NetworkToString(Reader);
                    Console.WriteLine($"End listen {pos}");
                }
                catch (Exception e)
                {
                    ExceptionResponser.Response(e);
                    canContinue = false;
                }
                if (!string.IsNullOrEmpty(information))
                {
                    counter++;
                    Request req = new Request(information);
                    Response res = new Response(req);

                    switch (req.Type)
                    {
                        case RequestType.POST:
                            if (!UserLogin.isValidLoginFromPOSTForm(req.Data))
                            {
                                res = new Response("401.html", 401, "Unauthorized");
                            }
                            break;
                        case RequestType.GET:
                            if (req.AbsoluteFilePath == "/images.html")
                            {
                                res = new Response("401.html", 401, "Unauthorized");
                            }
                            break;
                        default:
                            break;
                    }
                    res.SendAsync(client);
                    Console.WriteLine($"{pos} - {counter}\r\n{information} {res.DataString} \r\n");
                }
                else
                {
                    client.Close();
                    Console.WriteLine($"Client {pos} disconnected...");
                    canContinue= false;
                }
            }
        });
    }
    private static string? NetworkToString (NetworkStream stream)
    {
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
        return _encoder.GetString(memoryStream.ToArray());
    }
}