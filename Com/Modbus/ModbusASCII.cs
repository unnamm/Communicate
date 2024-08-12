using Com.Serial;
using System.IO.Ports;

namespace Com.Modbus
{
    public class ModbusASCII : SerialCommunicate
    {
        public ModbusASCII(string portName, int baudRate, int dataBits, Parity parity) : base(portName, baudRate, dataBits, parity)
        {
        }
    }
}
