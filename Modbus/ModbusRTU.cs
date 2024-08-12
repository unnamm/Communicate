using Serial;
using System.IO.Ports;

namespace Modbus
{
    public class ModbusRTU : SerialCommunicate
    {
        public ModbusRTU(string portName, int baudRate, int dataBits, Parity parity) : base(portName, baudRate, dataBits, parity)
        {
        }
    }
}
