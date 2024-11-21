using Com;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello");

        TcpCommunicate.TestPlay();

        Console.ReadLine();
    }

    class Run
    {
        public async void F()
        {
            //test
            TcpCommunicate tcp = new("127.0.0.1", 6053, 1000);
            await tcp.ConnectAsync();
            var v = await tcp.ReadAsync();
            Console.WriteLine(Encoding.UTF8.GetString(v));
        }
    }
}
