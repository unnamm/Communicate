using Builder.Interface;
using Builder.Packet;
using Com;
using Com.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder
{
    public class SampleTwoWayPacket : TwoWayPacket<double> //test packet1
    {
        public override string QueryCommand() => "get data {0}";

        public override string WriteCommand() => "set data {0}";

        protected override double Convert(string receiveData) => double.Parse(receiveData);
    }

    public class ReadIDNPacket : QueryPacket<string> //test packet2
    {
        public override string QueryCommand() => "*IDN?";

        protected override string Convert(string receiveData) => receiveData;
    }


    public class SampleDevice : TcpCommunicate, IProtocol //test device, need IProtocol
    {
        public SampleDevice(string ip, int port) : base(ip, port)
        {
        }

        public async Task<string> QueryAsync(string command)
        {
            var data = await base.QueryAsync(Encoding.UTF8.GetBytes(command));
            return Encoding.UTF8.GetString(data);
        }

        public Task WriteAsync(string command) =>
            base.WriteAsync(Encoding.UTF8.GetBytes(command)).AsTask();
    }

    public class BuilderTest
    {
        public static async void SampleTest() //run test
        {
            SampleDevice device = new("127.0.0.1", 6053);
            await device.ConnectAsync();

            SampleTwoWayPacket packet = new()
            {
                WriteParams = [15.5d],
                QueryParams = ["@101"],
            };

            Build build = new(device);
            build.Add(packet);

            await build.Write(); //set data 15.5

            build.Add(new ReadIDNPacket());

            await build.Query(); //get data @101 //*IDN?

            var doubleData = build.GetPacket<SampleTwoWayPacket>().GetData();
            var stringData = build.GetPacket<ReadIDNPacket>().GetData();

        }
    }
}
