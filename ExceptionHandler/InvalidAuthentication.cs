using System;
using System.IO;
using System.Text;

namespace NetworkSocket.ExceptionHandler
{
    public class Authentication
    {
        public string vari, index, exist, fileNotFound, Unauthorized, endLine, result;
        public Authentication()
        {
            vari = "HTTP/1.1 ";
            index = "index.html";
            exist = "200 OK";
            fileNotFound = "404 Not Found";
            Unauthorized = "401 Unauthorized";
            endLine = "\r\n";
            result = "";
        }
        public string parse(string request)
        {
            string[] subs = request.Split(' ');
            result = vari;
            if (subs[0] == "GET")
            {
                string fileName = "0";
                string[] splitFile = subs[1].Split('/');
                if (splitFile[0] == "")
                {
                    fileName = index;
                }
                else
                {
                    fileName = splitFile[0];
                }
                try
                {
                    File.ReadAllText(fileName);
                    result += exist;
                    result += endLine;
                }
                catch (FileNotFoundException e)
                {
                    // file not found
                    //ExceptionHandler.handle(e);
                    result += fileNotFound;
                    result += endLine;
                }
            } 
            if (subs[0] == "POST")
            {
                string uname = subs[1], psw = subs[2];
                if(uname == "admin" && psw == "123456")
                {
                    // Trả về nội dung
                }
                else{
                    result += Unauthorized;
                }
                result += endLine;
            }
            return result;
        }
    }
}