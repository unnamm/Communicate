using Com.Interface;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Modbus
{
    public class ModbusTest
    {
        public class SampleDevice : SerialCommunicate, ICommunicate
        {
            public SampleDevice(string portName, int baudRate, int dataBits, Parity parity, int timeout = 1000) :
                base(portName, baudRate, dataBits, parity, timeout)
            {
            }
        }

        public async static void SampleTestRTU()
        {
            var com = new SampleDevice("COM4", 9600, 8, Parity.None);
            await com.ConnectAsync();

            ModbusRTU rtu = new ModbusRTU(com);

            var data = await rtu.ReadInputRegisters(7600, 1);
        }
    }
}
