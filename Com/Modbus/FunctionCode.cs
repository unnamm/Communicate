using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Modbus
{
    public enum FunctionCode
    {
        None,
        ReadCoil = 0x01,
        ReadDiscreteInputs = 0x02,
        ReadHolingRegisters = 0x03,
        ReadInputRegisters = 0x04,
        WriteSingleCoil = 0x05,
        WriteSingleRegister = 0x06,
        WriteMultipleCoils = 0x0F,
        WriteMultipleRegisters = 0x10,
    }
}
