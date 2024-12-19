using Builder.Interface;
using Com;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run.Test.Device
{
    /// <summary>
    /// sample tcp device
    /// </summary>
    public class SampleDevice1 : TcpCommunicate, IProtocol //test device, need IProtocol
    {
        public SampleDevice1(string ip, int port) : base(ip, port)
        {
        }

        public async Task<string> QueryAsync(string command)
        {
            var data = await base.QueryAsync(Encoding.UTF8.GetBytes(command));
            return Encoding.UTF8.GetString(data);
        }

        public Task WriteAsync(string command) =>
            base.WriteAsync(Encoding.UTF8.GetBytes(command)).AsTask();
    }
}
