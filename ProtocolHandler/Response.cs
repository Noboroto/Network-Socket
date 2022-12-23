using System.IO;
using System.Linq;

using NetworkSocket.ProtocolHandler;

namespace NetworkSocket.ProtocalHandler
{
    public class Response : HttpProtocol
    {
        private int _statusCode;
        private string _statusMessage;

        public string StatusMessage => _statusMessage;
        public int StatusCode => _statusCode;

        /// <exception cref="FileNotFoundException"/>
        public Response (Request req):this(req.Target, 200, "OK", req.isKeepAlive)
        {

        }

        public Response(string filePath = "", int code = 200, string message = "OK", bool KeapAlive = true)
            : base($"HTTP/1.1 {code} {message}\r\n")
        {
            _statusCode = code;
            _statusMessage = message;

            if (!KeapAlive)
            {
                UpdateHeader("Connection", "close");
            }

            UpdateHeader("Content-Type", getContentType(filePath));
        }

        /// <exception cref="FileNotFoundException"/>
        public void ReplaceData(string path)
        {
            var data = File.ReadAllBytes(path);
            ReplaceData(data);
        }

        public void ReplaceTextData(string text)
        {
            ReplaceData(s_encoder.GetBytes(text));
        }

        private static string getContentType(string path)
        {
            string Result = "";
            var ext = path.Split(".").Last().ToLower();
            switch (ext)
            {
                case "html":
                case "htm":
                    Result += "text/html" + "; charset=UTF-8";
                    break;
                case "txt":
                    Result += "text/plan" + "; charset=UTF-8";
                    break;
                case "jpg":
                case "jpeg":
                    Result += "image/jpeg";
                    break;
                case "gif":
                    Result += "image/gif";
                    break;
                case "png":
                    Result += "image/png";
                    break;
                case "css":
                    Result += "text/css";
                    break;
                default:
                    Result += "application/octet-stream";
                    break;
            }
            return Result;
        }
        public override string ToString()
        {
            return StartLine;
        }
    }
}