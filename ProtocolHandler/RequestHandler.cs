using NetworkSocket.ProtocalHandler;
using NetworkSocket.User;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket.ProtocolHandler
{
    public static class RequestHandler
    {
        private static HashSet<string> s_blockPath = new HashSet<string>();

        private static Dictionary<int, string> DEFAULT_ERROR_PAGE = new Dictionary<int, string>
        {
            {401,"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title>401 Unauthorized</title>\r\n</head>\r\n<body>\r\n    <h1>401 Unauthorized</h1>\r\n    <p>This is a private area.</p>\r\n</body>\r\n\r\n</html>" },
            {404, "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title> 404 Not Found </title>\r\n</head>\r\n<body>\r\n    <h1>404 Not Found</h1>\r\n    <p>The requested file cannot be found.</p>\r\n</body>\r\n</html>"},
        };

        private static Dictionary<int, string> DEFAULT_ERROR_FILE = new Dictionary<int, string>
        {
            {401,"401.html" },
            {404,"404.html" }
        };

        private static Dictionary<int, string> DEFAULT_MESSAGE = new Dictionary<int, string>
        {
            {200, "OK" },
            {401, "Unauthorized"},
            {404, "File Not Found" }
        };

        public static Response? Handle(Request req, string SrcPath, UserDataContext? context = null)
        {
            switch (req.Type)
            {
                case RequestType.GET:
                    return HandlerGET(req, SrcPath);
                case RequestType.POST:
                    return HandlerPOST(req, SrcPath, context);
                default
                    : return null;
            }
        }

        private static Response GetError(int code, string SrcPath)
        {
            string message = "";
            if (DEFAULT_MESSAGE.ContainsKey(code))
                message = DEFAULT_MESSAGE[code];
            
            Response res = new Response($"{code}.html", code, message);
            if (DEFAULT_ERROR_FILE.ContainsKey(code))
            {
                string path = Path.Join(SrcPath, DEFAULT_ERROR_FILE[code]);
                path.Replace(@"/\", Path.PathSeparator.ToString())
                    .Replace(@"\\", Path.PathSeparator.ToString())
                    .Replace(@"//", Path.PathSeparator.ToString());

                if (File.Exists(path))
                    res.ReplaceData(path);
            }
            else if (DEFAULT_ERROR_PAGE.ContainsKey(code))
            {
                res.ReplaceData(DEFAULT_ERROR_PAGE[code]);
            }
            return res;
        }

        private static Response HandlerGET (Request req, string SrcPath)
        {
            Response? res = new Response(req);

            string path = Path.Join(SrcPath, req.Target);

            path.Replace(@"/\", Path.PathSeparator.ToString())
                .Replace(@"\\", Path.PathSeparator.ToString())
                .Replace(@"//", Path.PathSeparator.ToString());
            
            if (s_blockPath.Contains(req.Target) || s_blockPath.Contains(path))
            {
                return GetError(401, SrcPath);
            }

            if (!File.Exists(path))
            {
                return GetError(404, SrcPath);
            }

            res.ReplaceData (path);
            return res;
        }

        private static Response HandlerPOST (Request req, string SrcPath, UserDataContext? context)
        {
            bool isValid = false;
            if (context == null)
                return GetError(401, SrcPath);

            UserInfo? login = UserInfo.parse(req.TextData);

            if (login == null)
                return GetError(401, SrcPath);

            foreach (var user in context.UserInfos)
            {
                if (user.isValid(login))
                {
                    isValid = true;
                    break;
                }
            }

            if (!isValid)
            {
                return GetError(401, SrcPath);
            }

            s_blockPath.Add(req.Target);

            Response? res = new Response(req);
            string path = Path.Join(SrcPath, req.Target);

            path.Replace(@"/\", Path.PathSeparator.ToString())
                .Replace(@"\\", Path.PathSeparator.ToString())
                .Replace(@"//", Path.PathSeparator.ToString());
            
            if (!File.Exists(path))
            {
                return GetError(404, SrcPath);
            }

            res.ReplaceData(path);
            return res;
        }
    }
}
