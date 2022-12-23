using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket.User
{
    public class UserInfo
    {
        private string _username;
        private string _password;

        public UserInfo(string username, string password)
        {
            _username = username; 
            _password = password;
        }

        public bool isValid(UserInfo info)
        {
            return (info._username == _username && info._password == _password);
        }

        public override string ToString()
        {
            return $"uname={_username}$psw={_password}";
        }

        public static UserInfo? parse(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            var headers = data.Split("&");
            foreach (var header in headers)
            {
                var pair = header.Split("=");
                pairs[pair[0]] = pair[1];
            }

            return new UserInfo(pairs["uname"], pairs["psw"]);
        }
    }
}
