using Builder.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Packet
{
    internal abstract class WritePacket : IWritePacket
    {
        public object[]? WriteParams { get; set; }

        public abstract string WriteCommand();
    }
}
