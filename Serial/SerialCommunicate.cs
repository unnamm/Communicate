using Common;
using System.IO.Ports;

namespace Serial
{
    /// <summary>
    /// connect serial port
    /// </summary>
    public class SerialCommunicate : Communicate
    {
        private readonly SerialPort _serialPort;

        public SerialCommunicate(string portName, int baudRate, int dataBits, Parity parity)
        {
            _serialPort = new()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity
            };
        }

        protected override Task<Stream> connectAsync()
        {
            _serialPort.Open();
            return Task.FromResult(_serialPort.BaseStream);
        }

        public override void Dispose()
        {
            base.Dispose();
            _serialPort.Close();
        }
    }
}
