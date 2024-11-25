using Com.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Modbus
{
    public class ModbusTCP : Common.Modbus, IModbus
    {
        public ModbusTCP(ICommunicate c, bool isUseCRC = false) : base(c, isUseCRC)
        {
        }

        public Task<Dictionary<ushort, bool>> ReadCoils(ushort startAddress, ushort readNum, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<ushort, bool>> ReadDiscreteInputs(ushort startAddress, ushort readNum, byte slave = 1)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<ushort, ushort>> ReadHoldingRegisters(ushort startAddress, ushort readNum, byte slave = 1) =>
            ReadRegisters(0x03, startAddress, readNum, slave);

        public Task<Dictionary<ushort, ushort>> ReadInputRegisters(ushort startAddress, ushort readNum, byte slave = 1) =>
            ReadRegisters(0x04, startAddress, readNum, slave);

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

        public async void WriteSingleRegister(ushort address, ushort data, byte slave = 1)
        {
            var addresses = BitConverter.GetBytes(address);
            var datas = BitConverter.GetBytes(data);

            byte[] sendData =
            [
                0x00, 0x01, //Transaction Identifier, send == receive
                0x00, 0x00, //Protocol Identifier, fix 0000
                0x00, 0x06, //byte count after this
                slave,      //Unit Identifier, slave
                0x06,       //Function Code
                addresses[1], addresses[0], //Address
                datas[1], datas[0]          //value
            ];

            await _communicate.QueryAsync(sendData);
        }

        private async Task<Dictionary<ushort, ushort>> ReadRegisters(byte code, ushort startAddress, ushort readNum, byte slave)
        {
            var addresses = BitConverter.GetBytes(startAddress);
            var readNums = BitConverter.GetBytes(readNum);

            byte[] sendData =
            [
                0x00, 0x01, //Transaction Identifier, send == receive
                0x00, 0x00, //Protocol Identifier, fix 0000
                0x00, 0x06, //byte count after this
                slave,      //Unit Identifier, slave
                code,       //Function Code
                addresses[1], addresses[0], //Starting Address
                readNums[1], readNums[0]    //read num
            ];

            var receiveData = await _communicate.QueryAsync(sendData);

            Dictionary<ushort, ushort> dic = [];
            var byteCount = receiveData[8];
            for (ushort i = 0; i < byteCount; i += 2)
            {
                var value = BitConverter.ToUInt16([receiveData[10 + i], receiveData[9 + i]]);
                dic.Add((ushort)(startAddress + i), value);
            }

            return dic;
        }

    }
}
