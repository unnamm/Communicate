using Com.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Modbus
{
    public class ModbusTCP : IModbus
    {
        protected readonly ICommunicate _communicate;

        public ModbusTCP(ICommunicate c)
        {
            _communicate = c;
        }

        public Task<IEnumerable<bool[]>> ReadCoils(ushort startAddress, ushort readNum, byte slave = 1) =>
            ReadBits(0x01, startAddress, readNum, slave);

        public Task<IEnumerable<bool[]>> ReadDiscreteInputs(ushort startAddress, ushort readNum, byte slave = 1) =>
            ReadBits(0x02, startAddress, readNum, slave);

        public Task<IEnumerable<ushort>> ReadHoldingRegisters(ushort startAddress, ushort readNum, byte slave = 1) =>
            ReadRegisters(0x03, startAddress, readNum, slave);

        public Task<IEnumerable<ushort>> ReadInputRegisters(ushort startAddress, ushort readNum, byte slave = 1) =>
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
            var sendData = MakeSendData(0x06, address, data, slave);

            await _communicate.QueryAsync(sendData);
        }

        private async Task<IEnumerable<bool[]>> ReadBits(byte code, ushort startAddress, ushort readNum, byte slave)
        {
            var sendData = MakeSendData(code, startAddress, readNum, slave);

            var receiveData = await _communicate.QueryAsync(sendData);

            List<bool[]> bitDatas = [];

            var byteCount = receiveData[8];
            for (int i = 0; i < byteCount; i++)
            {
                var bitArray = new BitArray((byte[])[receiveData[9 + i]]);
                bool[] boolsFrombyte = new bool[8];
                bitArray.CopyTo(boolsFrombyte, 0);
                bitDatas.Add(boolsFrombyte);
            }

            return bitDatas;
        }

        private async Task<IEnumerable<ushort>> ReadRegisters(byte code, ushort startAddress, ushort readNum, byte slave)
        {
            var sendData = MakeSendData(code, startAddress, readNum, slave);

            var receiveData = await _communicate.QueryAsync(sendData);

            List<ushort> dic = [];
            var byteCount = receiveData[8];
            for (ushort i = 0; i < byteCount; i += 2)
            {
                var value = BitConverter.ToUInt16([receiveData[10 + i], receiveData[9 + i]]);
                dic.Add(value);
            }

            return dic;
        }

        private static List<byte> MakeSendData(byte code, ushort address, ushort data, byte slave)
        {
            var addresses = BitConverter.GetBytes(address);
            var datas = BitConverter.GetBytes(data);

            List<byte> sendData =
            [
                0x00, 0x01, //Transaction Identifier, send == receive
                0x00, 0x00, //Protocol Identifier, fix 0000
                0x00, 0x06, //byte count after this
                slave,      //Unit Identifier, slave
                code,       //Function Code
                addresses[1], addresses[0], //Starting Address
                datas[1], datas[0]    //read num or data
            ];
            return sendData;
        }

    }
}
