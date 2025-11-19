using Com.Common;
using System.IO.Ports;

namespace Com
{
    public class SerialCommunicate : Communicate
    {
        private readonly SerialPort _serialPort;

        public SerialCommunicate(
            string portName,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            SerialDataReceivedEventHandler? receiveHandler = null,
            int timeoutMilli = 1000
            ) : base(timeoutMilli)
        {
            _serialPort = new()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
            };

            if (receiveHandler != null)
            {
                _serialPort.DataReceived += receiveHandler;
            }
        }

        protected async override Task<Stream> ConnectAndStream()
        {
            await Task.Run(_serialPort.Open).Timeout(_timeout);
            return _serialPort.BaseStream;
        }

        public override void Dispose()
        {
            base.Dispose();
            _serialPort.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
