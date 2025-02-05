using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Interface
{
    public interface IQueryProtocol
    {
        Task<byte[]> QueryAsync(IEnumerable<byte> data, int readDelay = 0);
    }
}
