// See https://aka.ms/new-console-template for more information

using Com.Modbus;
using System.Collections;

Console.WriteLine("Hello, World!");

ModbusTCP tcp = new ModbusTCP("1", 1, 1000);

Console.ReadLine();
