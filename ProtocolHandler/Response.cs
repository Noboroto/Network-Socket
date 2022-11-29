using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using NetworkSocket.ExceptionHandler;
using NetworkSocket.SocketHandler;

namespace NetworkSocket.ProtocalHandler
{
    public enum RequestDataType
    {
        Empty,
        Html,
        Txt,
        Css,
        Jpeg,
        Gif,
        Png,
        Unknown
    }

    public class Response
    {
        private StringBuilder _headerBuilder = new StringBuilder();
        private RequestDataType _dataType;
        private int _statusCode;
        private string _statusMessage;
        private string? _dataFilePath;
        private bool _keepAlive;
        private static Encoding _encoder = Encoding.UTF8;

        public string? dataFilePath {get => _dataFilePath; private set => _dataFilePath = Directory.GetCurrentDirectory() + "/http/" + value;}

        public string DataString
        {
            get
            {
                var package = _encoder.GetBytes(_headerBuilder.ToString()).ToList();
                if (_dataType > RequestDataType.Empty && _dataType < RequestDataType.Jpeg && !string.IsNullOrEmpty(dataFilePath))
                    package.AddRange(File.ReadAllBytes(dataFilePath));
                return _encoder.GetString(package.ToArray());
            }
        }

        public Response (Request req):this(req.AbsoluteFilePath, 200, "OK", req.KeepAlive)
        {

        }

        private void BuildHeader()
        {
            _headerBuilder.Clear();
            _headerBuilder.Append("HTTP/1.1 ");
            _headerBuilder.Append(_statusCode);
            _headerBuilder.Append(" ");
            _headerBuilder.Append(_statusMessage);
            _headerBuilder.Append("\r\n");
            if (!_keepAlive) _headerBuilder.Append("Connection: close\r\n"); 
            _headerBuilder.Append(getContentTypeHeader(_dataType));
            if (_dataType != RequestDataType.Empty && !string.IsNullOrEmpty(dataFilePath))
            {
                FileInfo info = new FileInfo(dataFilePath);
                _headerBuilder.Append("Content-Length: ");
                _headerBuilder.Append(info.Length);
                _headerBuilder.Append("\r\n");
            }
            _headerBuilder.Append("\r\n");
        }
        public Response(string? filePath = "", int code = 200, string message = "OK", bool isKeepAlive = true)
        {
            dataFilePath = filePath;
            _dataType = getDataTypeByExtension(filePath);
            _statusCode = code;
            _statusMessage = message;
            _keepAlive = isKeepAlive;

            try
            {
                BuildHeader();
            }
            catch (FileNotFoundException)
            {
                dataFilePath = "404.html";
                _dataType = getDataTypeByExtension(dataFilePath);
                _statusCode = 404;
                _statusMessage = "Not Found";
                BuildHeader();
            }
        }

        private static RequestDataType getDataTypeByExtension(string? path)
        {
            if (string.IsNullOrEmpty(path)) return RequestDataType.Empty;
            var ext = path.Split(".").Last().ToLower();
            switch (ext)
            {
                case "html":
                case "htm":
                    return RequestDataType.Html;
                case "txt":
                    return RequestDataType.Txt;
                case "jpg":
                case "jpeg":
                    return RequestDataType.Jpeg;
                case "css":
                    return RequestDataType.Css;
                case "gif":
                    return RequestDataType.Gif;
                case "png":
                    return RequestDataType.Png;
                default:
                    return RequestDataType.Unknown;
            }
        } 
        private static string getContentTypeHeader(RequestDataType requestDataType)
        {
            string Result = "Content-Type: ";
            switch (requestDataType)
            {
                case RequestDataType.Empty:
                    return "";
                case RequestDataType.Html:
                    Result += "text/html" + "; charset=UTF-8";
                    break;
                case RequestDataType.Txt:
                    Result += "text/plan" + "; charset=UTF-8";
                    break;
                case RequestDataType.Jpeg:
                    Result += "image/jpeg";
                    break;
                case RequestDataType.Gif:
                    Result += "image/gif";
                    break;
                case RequestDataType.Png:
                    Result += "image/png";
                    break;
                case RequestDataType.Css:
                    Result += "text/css";
                    break;
                default:
                    Result += "application/octet-stream";
                    break;
            }
            return Result + "\r\n";
        }

        private void Send (ClientInfo client)
        {
            if (!client.isConnected) return;
            var stream = client.GetStream();
            if (_dataType != RequestDataType.Empty && !string.IsNullOrEmpty(dataFilePath))
            {
                var package = _encoder.GetBytes(_headerBuilder.ToString()).ToList();
                package.AddRange(File.ReadAllBytes(dataFilePath));
                if (!client.isConnected) 
                    return;
                stream.Write(package.ToArray());
            }
            else
            {
                _headerBuilder.Append("\r\n");
                stream.Write(_encoder.GetBytes(_headerBuilder.ToString()));
            }
            stream.Flush();
        }

        public void SendAsync (ClientInfo client)
        {
            Task.Run(() => Send(client));
        }
    }
}