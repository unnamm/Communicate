using Com;
using Com.Modbus;
using Com.Packet;
using Run.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello");

        _ = new Run().Func();

        Console.ReadLine();
    }

    class Run
    {
        public async Task Func()
        {
            try
            {
                SerialCommunicate sc = new("COM3");
                await sc.ConnectAsync();
                var receive = await sc.QueryAsync([]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
