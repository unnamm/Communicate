using Com.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Run.Test.Packet
{
    internal partial class QuerySamplePacket : QueryPacket<double[]>
    {
        [GeneratedRegex(@"\d*\.?\d+")]
        private static partial Regex RealNumber();

        public override string GetCommand() => "#0{0}\r";

        protected override double[] Convert(string receiveData) =>
            RealNumber().Matches(receiveData).Select(x => double.Parse(x.Value)).ToArray();
    }
}
