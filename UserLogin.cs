using NetworkSocket.ProtocalHandler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket
{
    internal class UserLogin
    {
        public static bool isValidLoginFromPOSTForm (String? data)
        {
            if (string.IsNullOrEmpty(data)) return true;
            Dictionary <string, string> pairs = new Dictionary <string, string> ();
            var headers = data.Split("&");
            foreach(var header in headers)
            {
                var pair = header.Split("=");
                pairs[pair[0]] = pair[1];
            }
            return ((pairs["uname"] == "admin") && pairs["psw"] == "123456");
        }
    }
}
