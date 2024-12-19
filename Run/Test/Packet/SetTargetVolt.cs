using Builder.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run.Test.Packet
{
    /// <summary>
    /// setting target volt
    /// </summary>
    internal class SetTargetVolt : TwoWayPacket<double>
    {
        public override string QueryCommand() => "get data"; //get targetvolt

        public override string WriteCommand() => "set data {0}"; //set targetvolt

        protected override double Convert(string receiveData) => double.Parse(receiveData);
    }
}
