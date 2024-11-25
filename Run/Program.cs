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

        Builder.BuilderTest.SampleTest();

        Console.ReadLine();
    }
}
