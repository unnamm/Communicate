using Com.Interface;

namespace Com.Modbus
{
    public class ModbusASCII : IModbus
    {
        public Task<Dictionary<ushort, bool>> ReadCoils(ushort startAddress, ushort readNum, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<ushort, bool>> ReadDiscreteInputs(ushort startAddress, ushort readNum, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<ushort, ushort>> ReadHoldingRegisters(ushort startAddress, ushort readNum, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<ushort, ushort>> ReadInputRegisters(ushort startAddress, ushort readNum, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public void WriteMultipleCoils(ushort startAddress, IEnumerable<bool> values, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public void WriteMultipleRegisters(ushort address, IEnumerable<ushort> data, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public void WriteSingleCoil(ushort address, bool value, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public void WriteSingleRegister(ushort address, ushort data, byte slave = 1)
        {
            throw new NotImplementedException();
        }
    }
}
