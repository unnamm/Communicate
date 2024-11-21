using Com.Common;
using System.IO.Ports;

namespace Com
{
    public class SerialCommunicate : Communicate
    {
        private readonly SerialPort _serialPort;

        public SerialCommunicate(string portName, int baudRate, int dataBits, Parity parity, int timeout = 1000) :
            base(timeout)
        {
            _serialPort = new()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity
            };
        }

        protected async override Task<Stream> GetStreamAfterConnect()
        {
            await Task.Run(_serialPort.Open).Timeout(_timeout);
            return _serialPort.BaseStream;
        }

        public override void Dispose()
        {
            base.Dispose();
            _serialPort.Dispose();
        }
    }
}
