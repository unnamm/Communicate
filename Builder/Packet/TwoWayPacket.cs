using Builder.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Packet
{
    /// <summary>
    /// query and write packet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TwoWayPacket<T> : QueryPacket<T>, IWritePacket
    {
        public object[]? WriteParams { get; set; }

        public abstract string WriteCommand();
    }
}
