using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

// Reference: https://docs.codingtipi.com/docs/toolkit/http-request-handler/ctors


namespace NetworkSocket.ProtocolHandler
{
    internal class HttpMethods
    {
        public const string Get = "GET";
        public const string Post = "POST";
    }
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Body { get; set; } = default!;
    }
    public class RequestHandler
    {
        #region Attribute
        private readonly HttpClient m_client;
        private HttpResponseMessage? m_response;
        private StringContent? m_information;

        #endregion

        #region Constructor 
        public RequestHandler()
        {
            m_client = new HttpClient();
        }

        public RequestHandler(string Token) : this()
        {
            m_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }

        // This constructor initializes an HttpClient class with aditional headers provided as Key Value from a dictionary.
        public RequestHandler(Dictionary<string, string> headers) : this()
        {
            foreach (var header in headers)
            {
                m_client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        // Combine above
        public RequestHandler(string bearerToken, Dictionary<string, string> headers) : this()
        {
            m_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            foreach (var header in headers)
            {
                m_client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        #endregion
        
        #region Method
        public async Task<HttpResponse> ExecuteAsync(string method, string endpoint, string? body = null)
        {
            m_response = new HttpResponseMessage(); //Initialize Response object);

            if (body != null) //Check if the execute request need a base model for the body parameter
            {
                fixBody(body);
            }
            if (method == HttpMethods.Post)
            {
                m_response = await m_client.PostAsync(endpoint, m_information);
            }
            else if (method == HttpMethods.Get)
            {
                m_response = await m_client.GetAsync(endpoint);
            }
            return new HttpResponse
            {
                StatusCode = m_response.StatusCode,
                Body = await m_response.Content.ReadAsStringAsync()
            };
        }

        public void Dispose()
        {
            m_client.Dispose();
            if (m_response != null)
            {
                m_response.Dispose();
            }
            if (m_information != null)
            {
                m_information.Dispose();
            }
        }

        #endregion

        #region Body
        private void fixBody(string jsonObject)
        {
            m_information = new StringContent(jsonObject, Encoding.UTF8, "application/json");
        }
        #endregion
    }
}