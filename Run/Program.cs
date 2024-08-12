// See https://aka.ms/new-console-template for more information
using System.Text;
using Tcp;

Console.WriteLine("Hello, World!");

Run();

Console.ReadLine();

async void Run()
{
    //test
    while(true)
    {
        TcpCommunicate tc = new("127.0.0.1", 5000, 999999, 100);
        await tc.InitAsync();
        var v = await tc.ReadAsync();
        var str = Encoding.UTF8.GetString(v);
        Console.WriteLine(str);
        tc.Dispose();
    }
}