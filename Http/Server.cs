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
            using var response = context.Response;

            byte[] respByte; //send data

            var rawUrl = request.RawUrl!.Replace("/", string.Empty);

            if (string.IsNullOrEmpty(rawUrl)) //post
            {
                using var body = request.InputStream;
                using var reader = new StreamReader(body, Encoding.UTF8);
                var data = await reader.ReadToEndAsync(); //body data

                respByte = Encoding.UTF8.GetBytes("post");
            }
            else //get
            {
                respByte = rawUrl switch  //send message
                {
                    "get" => Encoding.UTF8.GetBytes(RESPONSE),
                    "get2" => Encoding.UTF8.GetBytes("response get2"),
                    _ => throw new NotImplementedException()
                };
            }

            //response.ContentType = "text/html";
            //response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = respByte.LongLength;

            await response.OutputStream.WriteAsync(respByte);
        }
    }
}
