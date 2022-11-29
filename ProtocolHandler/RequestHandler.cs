using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetworkSocket.ProtocolHandler
{
    internal class HttpMethods
    {
        public const string Get = "GET";
        public const string Post = "POST";
    }

    public class RequestHandler
    {
        public bool method_get = true;
        public string m_filename = " ";
        public string m_endpoint = " ";
        public bool keep_alive = true;
        public RequestHandler(byte[] bin_sequence)
        {
            byte[] blob = new byte[bin_sequence.Length / 2];
            string res = Encoding.Unicode.GetString(blob);
            getInfo(res);

        }

        public void getInfo(string arg)
        {
            var sub = arg.Split("\r\n");

            //get method
            if (sub[0].Contains("POST"))
            {
                method_get = false;
            }

            // get filename
            var subsub = sub[0].Split(" ");
            m_filename = "http/" + subsub[1];

            if (m_filename == "/")
            {
                m_filename = "/http/index.html";
            }

            // add directory
            m_filename = Directory.GetCurrentDirectory() + m_filename;

            //Define connection
            for (int i = 0; i < sub.Length; i++)
            {
                if (sub[i].Contains("Connection"))
                {
                    if (sub[i].Contains("close"))
                    {
                        keep_alive = false;
                        break;
                    }
                }
            }
        }
    }
}