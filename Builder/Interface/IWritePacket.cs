using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Interface
{
    internal interface IWritePacket : IPacket
    {
        /// <summary>
        /// write command parameters
        /// </summary>
        object[]? WriteParams { get; set; }

        /// <summary>
        /// string.format command, params are {0} {1} ...
        /// </summary>
        /// <returns></returns>
        string WriteCommand();
    }
}
