using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetworkSocket.User
{
    public class UserDataContext
    {
        private List<UserInfo> _userInfos;
        private const string _filePath = "./User.txt";

        public List<UserInfo> UserInfos => _userInfos;
        public UserDataContext() 
        {
            _userInfos = new List<UserInfo>();

            if (!File.Exists(_filePath)) return;

            var lines = File.ReadAllLines(_filePath);
            foreach (var line in lines)
            {
                var user = UserInfo.parse(line);
                if (user == null) continue;
                _userInfos.Add(user);
            }
        }

        public void AddUser (UserInfo user)
        {
            _userInfos.Add(user);
            File.AppendAllText(_filePath, user.ToString() + "\n", Encoding.UTF8);
        }
    }
}
