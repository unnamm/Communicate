using Builder;
using Com.Common;
using Run.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello");

        BuilderTest.Run();

        Console.ReadLine();
    }
}
