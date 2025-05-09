using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Http
{
    public class Client
    {
        private readonly HttpClient _client = new();

        public async void Get(string rawUrl)
        {
            Stopwatch sw = Stopwatch.StartNew();
            using var response = await _client.GetAsync($"http://localhost/{rawUrl}/"); //address/rawurl/
            var message = await GetMessage(response);
            Console.WriteLine($"response message= {message}\nresponse time= {sw.Elapsed}");
        }

        public async void Post()
        {
            var content = JsonSerializer.Serialize(new
            {
                userId = 77,
                id = 1,
                title = "write code sample",
                completed = false
            });

            using StringContent jsonContent = new(content, Encoding.UTF8);
            Stopwatch sw = Stopwatch.StartNew();
            using var response = await _client.PostAsync("http://localhost", jsonContent);
            //using var response = await _client.PostAsJsonAsync("http://localhost", new Point() { X = 1, Y = 2 }); //auto make json
            var message = await GetMessage(response);
            Console.WriteLine($"response message= {message}\nresponse time= {sw.Elapsed}");
        }

        private static Task<string> GetMessage(HttpResponseMessage message)
        {
            using var response = message.EnsureSuccessStatusCode(); //if not successful, error occurs
            return response.Content.ReadAsStringAsync(); //get message
        }
    }
}
