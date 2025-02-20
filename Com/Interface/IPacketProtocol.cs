using Com.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Interface
{
    public interface IPacketProtocol : IDisposable
    {
        Task ConnectAsync();
        Task<T> QueryAsync<T>(QueryPacket<T> packet, params object[] @params);
        ValueTask WriteAsync(WritePacket packet, params object[] @params);
    }
}
