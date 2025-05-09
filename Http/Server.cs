using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Http
{
    public class Server
    {
        public const string RESPONSE = """
        [
          {
            userId: 1,
            id: 1,
            title: "example title",
            completed: false
          },
          {
            userId: 1,
            id: 2,
            title: "another example title",
            completed: true
          },
        ]
        """;

        public async void RunListen()
        {
            HttpListener listener = new();
            listener.Prefixes.Add("http://localhost/");

            try
            {
                listener.Start();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("need administrator rights");
                throw;
            }

            var context = await listener.GetContextAsync(); //when until message is received from client

            var request = context.Request;
            byte[]? respByte = null; //send data

            if (request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) //get
            {
                var rawUrl = request.RawUrl!.Replace("/", string.Empty);

                if (rawUrl.Equals("get", StringComparison.OrdinalIgnoreCase))
                {
                    respByte = Encoding.UTF8.GetBytes(RESPONSE);
                }
                else if (rawUrl.Equals("get2", StringComparison.OrdinalIgnoreCase))
                {
                    respByte = Encoding.UTF8.GetBytes("response get2");
                }
            }
            else if (request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)) //post
            {
                using var body = request.InputStream;
                using var reader = new StreamReader(body, Encoding.UTF8);
                var data = reader.ReadToEnd(); //body data

                Console.WriteLine($"message received by server is= {data}");

                respByte = Encoding.UTF8.GetBytes("response post");
            }

            if (respByte == null)
            {
                throw new NotImplementedException();
            }

            using var response = context.Response;
            //response.ContentType = "text/html";
            //response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = respByte.LongLength;

            await response.OutputStream.WriteAsync(respByte); //send client
        }
    }
}
