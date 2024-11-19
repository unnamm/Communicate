using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Hello");

        var t = new Test();
        var t2 = new Test<int>();
    }

    class Test
    {
        public void F() { }
    }

    class Test<T>
    {

    }


}
