using Com.Common;
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


        byte data = 0x0f;

        List<bool> list = [];

        var bitArray = new BitArray(data);
        foreach (var bit in bitArray)
        {
            list.Add((bool)bit);
        }



        Console.ReadLine();
    }
}
