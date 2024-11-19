using Com.Common;

namespace Protocol.Modbus
{
    public class ModbusASCII : Modbus
    {
        public ModbusASCII(Communicate c) : base(c)
        {
        }

        public override Task<ushort[]> ReadRegisters(FunctionCode code, ushort address, ushort readNum, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public override Task WriteSingleRegister(ushort address, ushort value, byte slave = 1)
        {
            throw new NotImplementedException();
        }
    }
}
