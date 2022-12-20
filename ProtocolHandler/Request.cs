using System.Diagnostics;
using System.Linq;

namespace NetworkSocket.ProtocalHandler
{
    public enum RequestType
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public class Request
    {
        private string _filePath;
        private bool _KeepAlive;
        private RequestType _type;
        private string? _data;

        public string AbsoluteFilePath => _filePath;
        public bool KeepAlive => _KeepAlive;
        public RequestType Type => _type;

        public string? Data => _data;

        public Request(string raw)
        {
            var sentences = raw.Split("\r\n");
            var requestHeader = sentences[0].Split(" ");
            _filePath = requestHeader[1];
            if (_filePath == "/") _filePath = "index.html";
            _type = getTypeFormString(requestHeader[0]);
            foreach (var item in sentences)
            {
                var part = item.Split(':');
                if (part[0] == "Connection")
                {
                    if (part[1] == "close") _KeepAlive = false;
                    else _KeepAlive = true;
                }
            }

            _data = sentences.Last();
        }

        private RequestType getTypeFormString(string raw)
        {
            switch (raw)
            {
                case "GET":
                    return RequestType.GET;
                case "POST": 
                    return RequestType.POST;
                case "PUT": 
                    return RequestType.PUT;
                case "DELETE": 
                    return RequestType.DELETE;
                default:
                    return RequestType.GET;
            }
        }
    }
}