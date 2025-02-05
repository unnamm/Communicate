using Com.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Run.Test.Packet
{
    internal class QuerySamplePacket : QueryPacket<double[]>
    {
        public override string GetCommand()
        {
            return "#0{0}\r";
        }

        protected override double[] Convert(string receiveData)
        {
            return new Regex(@"\d*\.?\d+").Matches(receiveData).Select(x => double.Parse(x.Value)).ToArray();
        }
    }
}
