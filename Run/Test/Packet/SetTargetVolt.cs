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
    internal class SetTargetVolt : WritePacket
    {
        public override string GetCommand() => "set volt: {0}";
    }
}
