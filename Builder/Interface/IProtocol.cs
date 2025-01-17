using Builder.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Interface
{
    /// <summary>
    /// Device interface
    /// </summary>
    public interface IProtocol
    {
        Task<T> QueryAsync<T>(QueryPacket<T> packet, params object[] @params);
        Task WriteAsync(WritePacket packet, params object[] @params);
    }
}
