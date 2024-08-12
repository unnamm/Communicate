// See https://aka.ms/new-console-template for more information
using Serial;
using System.Text;
using Tcp;

Console.WriteLine("Hello, World!");

Run();

Console.ReadLine();

async void Run()
{
    TcpCommunicate t = new("127.0.0.1", 9999);
    await t.ConnectAsync();
    Console.WriteLine("end");
}