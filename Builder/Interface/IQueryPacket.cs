using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Interface
{
    internal interface IQueryPacket : IPacket
    {
        /// <summary>
        /// query command parameters
        /// </summary>
        object[]? QueryParams { get; set; }

        /// <summary>
        /// receive data after write query
        /// </summary>
        string? ReceiveData { get; set; }

        /// <summary>
        /// string.format command, params are {0} {1} ...
        /// </summary>
        /// <returns></returns>
        string QueryCommand();
    }
}
