using Builder.Interface;
using Com;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run.Test.Device
{
    public class SampleDevice2 : SerialCommunicate, IProtocol
    {
        public SampleDevice2(string portName, int baudRate, int dataBits, Parity parity, int timeout = 1000) :
            base(portName, baudRate, dataBits, parity, timeout)
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
