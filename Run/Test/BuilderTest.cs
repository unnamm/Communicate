using Builder;
using Com;
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
            TcpCommunicate tcp = new("127.0.0.1", 6053);

            Device device1 = new(tcp);
            await device1.ConnectAsync();

            await device1.WriteAsync(new SetTargetVolt(), "15.5");
            var idnData = await device1.QueryAsync(new ReadIDN()); //return string
            var sample = await device1.QueryAsync(new QuerySamplePacket(), 1); //return double[]

            device1.Dispose();

            //serial test
            SerialCommunicate serial = new("COM1", 9600, 8, System.IO.Ports.Parity.None);

            device1 = new(serial); //same device, other communicate
            await device1.ConnectAsync();

            await device1.WriteAsync(new SetTargetVolt(), "15.5");
            idnData = await device1.QueryAsync(new ReadIDN());
            sample = await device1.QueryAsync(new QuerySamplePacket(), 1);

            device1.Dispose();
        }
    }
}
