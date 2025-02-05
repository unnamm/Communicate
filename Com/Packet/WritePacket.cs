using Com.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Packet
{
    public abstract class WritePacket : IPacket
    {
        public object[]? Params { get; set; }

        public abstract string GetCommand();
    }
}
