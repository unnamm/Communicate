using Builder;
using Builder.Interface;
using Builder.Packet;
using Com;
using Com.Common;
using Run.Test.Device;
using Run.Test.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run.Test
{
    public class BuilderTest
    {
        public static async void Run() //run test
        {
            SampleDevice1 device1 = new("127.0.0.1", 6053);
            await device1.ConnectAsync();

            await RunAsync(new(device1)); //build match device

            SampleDevice2 device2 = new("COM1", 9600, 8, System.IO.Ports.Parity.None);
            await device2.ConnectAsync();

            await RunAsync(new(device2)); //same packet, other device
        }

        private static async Task RunAsync(Build build)
        {
            build.Add(new ReadIDN()); //no param packet
            build.Add(new SetTargetVolt() { WriteParams = [12d] }); //need write params

            await build.Write(); //write volt 12
            await build.Query(); //get idn, target volt

            //result query
            var doubleData = build.GetPacket<SetTargetVolt>().GetData();
            var stringData = build.GetPacket<ReadIDN>().GetData();
        }
    }
}
