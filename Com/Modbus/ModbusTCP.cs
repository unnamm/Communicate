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
        public ModbusTCP(string ip, int port) : base(ip, port)
        {

        }

        public async Task<ushort[]> ReadInputRegisters(ushort startingAddress, int readNum)
        {
            var add = BitConverter.GetBytes(startingAddress);
            var num = BitConverter.GetBytes(readNum);

            byte[] sendBuff =
            [
                0x00, 0x01, // Transaction Identifier
                0x00, 0x00, // Protocol Identifier
                0x00, 0x06, // Length
                0x01,       // Unit Identifier
                (byte)FunctionCode.ReadInputRegisters, // Function Code (Read Input Registers)
                add[0], add[1], // Starting Address
                num[0], num[1] // read num
            ];

            var receive = await QueryAsync(sendBuff);

            //var transactionId = BitConverter.ToUInt16([receive[0], receive[1]]);
            //var protocolId = BitConverter.ToUInt16([receive[2], receive[3]]);
            //var length = BitConverter.ToUInt16([receive[4], receive[5]]);
            //var unitId = receive[6];
            //var functionCode = (FunctionCode)receive[7];
            var byteCount = receive[8];
            var receiveData = new ushort[byteCount / 2];

            for (int i = 0; i < receiveData.Length; i++)
            {
                receiveData[i] = BitConverter.ToUInt16([receive[8 + i * 2], receive[9 + i * 2]]);
            }
            return receiveData;
        }
    }
}
