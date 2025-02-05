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
    public class BuilderTest
    {
        public static async void Run() //run test
        {
            //tcp test
            TcpCommunicate device1 = new("127.0.0.1", 6053);
            await device1.ConnectAsync();

            await device1.WriteAsync(new SetTargetVolt(), "15.5");
            var idnData = await device1.QueryAsync(new ReadIDN()); //return string
            var sample = await device1.QueryAsync(new QuerySamplePacket(), 1); //return double[]

            device1.Dispose();

            //serial test
            SerialCommunicate device2 = new("COM1", 9600, 8, System.IO.Ports.Parity.None);
            await device2.ConnectAsync();

            await device2.WriteAsync(new SetTargetVolt(), "15");
            idnData = await device2.QueryAsync(new ReadIDN());
            sample = await device2.QueryAsync(new QuerySamplePacket(), 1);

            device2.Dispose();

            //modbus test
            ModbusTCP modbus = new(device1);
            await device1.ConnectAsync();
            var result = await modbus.ReadHoldingRegisters(0x0000, 1, 1);
        }
    }
}
