using NetworkSocket.ProtocolHandler;

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
    public class Request : HttpProtocol
    {
        private string _target;
        private RequestType _type;

        public string Target => _target;
        public RequestType Type => _type;

        public Request(byte[] raw) : this (s_encoder.GetString(raw))
        {

        }

        public Request(string raw) : base (raw)
        {
            var requestHeader = StartLine.Split(" ");
            _target = "";

            if (string.IsNullOrEmpty(StartLine)) 
                return;

            _target = requestHeader[1].Split("?").First();
            if (_target == "/") _target = "index.html";
            _type = getTypeFormString(requestHeader[0]);
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
        public override string ToString()
        {
            string result = StartLine;
            if (Data != null && Data.Length > 0)
            {
                result += "\n" + TextData;
            }
            return result;
        }
    }
}