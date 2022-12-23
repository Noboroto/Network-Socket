using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;

namespace NetworkSocket.ProtocolHandler
{
    public class HttpProtocol
    {
        private Dictionary<string, string> _headers;
        private string _startLine;
        private byte[]? _data;

        protected static Encoding s_encoder = Encoding.UTF8;

        public bool isKeepAlive
        {
            get
            {
                if (Headers.ContainsKey("Connection"))
                {
                    if (Headers["Connection"] == "close")
                        return false;
                    else return true;
                }
                else return false;
            }
        }
        public string StartLine => _startLine;
        public string TextData
        {
            get
            {
                if (_data?.Length > 0)
                    return s_encoder.GetString(_data);
                return "";
            }
        }
        public byte[]? Data => _data;
        public Dictionary<string, string> Headers => new Dictionary<string, string>(_headers);
        protected HttpProtocol(string raw) 
        { 
            _headers = new Dictionary<string, string>();
            _data = null;
            _startLine = "";

            if (String.IsNullOrEmpty(raw)) return;

            var parts = raw.Split("\r\n\r\n");
            if (parts.Length > 1)
            {
                _data = s_encoder.GetBytes(parts.Last());
            }
            var sentences = parts[0].Split("\r\n");
            _startLine = sentences.First();
            for (int i = 1; i < sentences.Length; ++i)
            {
                if (string.IsNullOrEmpty(sentences[i])) continue;
                var header = sentences[i].Split(": ");
                _headers.Add(header[0], header[1]);
            }
        }

        protected void UpdateHeader(string key, string value)
        {
            _headers[key] = value;
        }

        protected void RepalceData (string data)
        {
            _data = s_encoder.GetBytes(data);
        }

        protected void ReplaceData(byte[] data)
        {
            _data = data;
            _headers["Content-Length"] = data.Length.ToString();
        }

        public byte[] getFullByte()
        {
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append(StartLine);
            headerBuilder.Append("\r\n");
            foreach (var item in Headers)
            {
                headerBuilder.Append(item.Key);
                headerBuilder.Append(": ");
                headerBuilder.Append(item.Value);
                headerBuilder.Append("\r\n");
            }
            headerBuilder.Append("\r\n");
            var result = s_encoder.GetBytes(headerBuilder.ToString()).ToList();
            if (Data != null) result.AddRange(Data);

            return result.ToArray();
        }
    }
}
