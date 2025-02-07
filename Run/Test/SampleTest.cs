using Com;
using Com.Modbus;
using Run.Test.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run.Test
{
    /// <summary>
    /// how to use
    /// </summary>
    public class SampleTest
    {
        public static async void Run() //run test
        {
            Test t = new();

            await t.Tcp();
            await t.Serial();
            await t.ModbusTcp();
            await t.ModbusRTU();
        }

        class Test
        {
            public async Task Tcp()
            {
                TcpCommunicate device = new("127.0.0.1", 6053);
                await device.ConnectAsync();

                await device.WriteAsync(new SetTargetVolt(), "15.5");
                var idnData = await device.QueryAsync(new ReadIDN()); //return string
                var sample = await device.QueryAsync(new QuerySamplePacket(), 1); //return double[]

                device.Dispose();
            }

            public async Task Serial() //same packet other protocol
            {
                SerialCommunicate device = new("COM1", 9600, 8, System.IO.Ports.Parity.None);
                await device.ConnectAsync();

                await device.WriteAsync(new SetTargetVolt(), "15");
                var idnData = await device.QueryAsync(new ReadIDN());
                var sample = await device.QueryAsync(new QuerySamplePacket(), 1);

                device.Dispose();
            }

            public async Task ModbusTcp()
            {
                TcpCommunicate tcp = new("localhost", 1234);
                await tcp.ConnectAsync();
                ModbusTCP modbus = new(tcp);
                var result = await modbus.ReadHoldingRegisters(0x0000, 1, 1);
            }

            public async Task ModbusRTU()
            {
                SerialCommunicate serial = new("COM2", 9600, 8, System.IO.Ports.Parity.None);
                await serial.ConnectAsync();
                ModbusRTU modbus = new(serial, false);
                var result = await modbus.ReadHoldingRegisters(0x1000, 10, 1);
            }
        }
    }
}
