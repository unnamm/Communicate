using Com.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.Modbus
{
    public class ModbusTCP : TcpCommunicate
    {
        public ModbusTCP(string ip, int port, int timeout = 1000) : base(ip, port, timeout)
        {

        }

        public async Task<ushort[]> ReadInputRegisters(ushort startingAddress, ushort readNum)
        {
            var sendBuff = makeSendData(FunctionCode.ReadInputRegisters, startingAddress, readNum);

            var receive = await QueryAsync(sendBuff);

            //var transactionId = BitConverter.ToUInt16([receive[0], receive[1]]);
            //var protocolId = BitConverter.ToUInt16([receive[2], receive[3]]);
            //var length = BitConverter.ToUInt16([receive[4], receive[5]]);
            //var unitId = receive[6];
            //var functionCode = (FunctionCode)receive[7];
            var byteCount = receive[8];
            var receiveDatas = new ushort[byteCount / 2];

            for (int i = 0; i < receiveDatas.Length; i++)
            {
                receiveDatas[i] = BitConverter.ToUInt16([receive[9 + i * 2], receive[8 + i * 2]]);
            }
            return receiveDatas;
        }

        public async void WriteSingleRegister(ushort startingAddress, ushort value)
        {
            var sendBuff = makeSendData(FunctionCode.ReadInputRegisters, startingAddress, value);

            _ = await QueryAsync(sendBuff);
        }

        private byte[] makeSendData(FunctionCode code, ushort startAddress, ushort value)
        {
            var addresses = BitConverter.GetBytes(startAddress);
            var values = BitConverter.GetBytes(value);

            byte[] sendBuff =
            [
                0x00, 0x01, // Transaction Identifier
                0x00, 0x00, // Protocol Identifier
                0x00, 0x06, // Length
                0x01,       // Unit Identifier
                (byte)code, // Function Code
                addresses[1], addresses[0], // Starting Address
                values[1], values[0] // read num
            ];
            return sendBuff;
        }
    }
}
