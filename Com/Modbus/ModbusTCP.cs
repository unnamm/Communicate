using Com.Tcp;
using System.Collections;

namespace Com.Modbus
{
    public class ModbusTCP : Modbus
    {
        public ModbusTCP(TcpCommunicate c) : base(c)
        {
        }

        public async Task<BitArray> ReadBits(FunctionCode code, ushort address, ushort readNum)
        {
            if (code != FunctionCode.ReadCoil && code != FunctionCode.ReadDiscreteInputs)
            {
                throw new Exception("incorrect code");
            }

            var send = makeSendData6(code, address, readNum);

            var read = await query(send);

            return getBits(read);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">ReadHolingRegisters, ReadInputRegisters</param>
        /// <param name="address"></param>
        /// <param name="readNum"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ushort[]> ReadRegisters(FunctionCode code, ushort address, ushort readNum)
        {
            if (code != FunctionCode.ReadHolingRegisters && code != FunctionCode.ReadInputRegisters)
            {
                throw new Exception("incorrect code");
            }

            var send = makeSendData6(code, address, readNum);

            var read = await query(send);

            return getUshorts(read);
        }

        //public async Task WriteSingleCoil(ushort address, bool value)
        //{
        //    var send = makeSendData6(FunctionCode.WriteSingleCoil, address, value ? (ushort)0xFF00 : (ushort)0x0000);

        //    await query(send);
        //}

        public async Task WriteSingleRegister(ushort address, ushort value)
        {
            var send = makeSendData6(FunctionCode.WriteSingleRegister, address, value);

            await query(send);
        }

        /// <summary>
        /// get bit array from receive data
        /// </summary>
        /// <param name="read">receive data</param>
        /// <returns></returns>
        private static BitArray getBits(byte[] read)
        {
            var byteCount = read[8];

            List<byte> list = [];

            for (int i = 0; i < byteCount; i++)
            {
                list.Add(read[9 + i]);
            }
            return new BitArray(list.ToArray());
        }

        /// <summary>
        /// get data array from receive data
        /// </summary>
        /// <param name="read">receive data</param>
        /// <returns></returns>
        private static ushort[] getUshorts(byte[] read)
        {
            var receiveDatas = new ushort[read[8] / 2];

            for (int i = 0; i < receiveDatas.Length; i++)
            {
                receiveDatas[i] = BitConverter.ToUInt16([read[10 + i * 2], read[9 + i * 2]]);
            }
            return receiveDatas;
        }

        /// <summary>
        /// make request one data
        /// </summary>
        /// <param name="code"></param>
        /// <param name="startAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte[] makeSendData6(FunctionCode code, ushort startAddress, ushort value)
        {
            var transaction = BitConverter.GetBytes(_transactionId);
            var addresses = BitConverter.GetBytes(startAddress);
            var values = BitConverter.GetBytes(value);

            byte[] sendBuff =
            [
                transaction[1], transaction[0], //Transaction Identifier, read write pair
                0x00, 0x00, //Protocol Identifier, 0000 fix
                0x00, 0x06, //Length
                0x01,       //Unit Identifier, slave
                (byte)code, //Function Code
                addresses[1], addresses[0], //Starting Address
                values[1], values[0] //read num, value
            ];

            return sendBuff;
        }

        /// <summary>
        /// make request multiple data
        /// </summary>
        /// <param name="code"></param>
        /// <param name="startAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        //private byte[] makeSendData(FunctionCode code, ushort startAddress, bool[] values)
        //{
        //    var transaction = BitConverter.GetBytes(_transactionId);
        //    var addresses = BitConverter.GetBytes(startAddress);

        //    var bits = new BitArray(values); //bool[] -> bit array
        //    var bytes = new byte[bits.Length / 8]; //8bit == 1byte
        //    bits.CopyTo(bytes, 0);

        //    var lengths = BitConverter.GetBytes((ushort)(7 + bytes.Length));
        //    var dataLengths = BitConverter.GetBytes((ushort)bits.Length); //bit count

        //    byte[] sendBuff =
        //    [
        //        transaction[1], transaction[0], //Transaction Identifier, read write pair
        //        0x00, 0x00, //Protocol Identifier, 0000 fix
        //        lengths[1], lengths[0], //Length
        //        0x01,       //Unit Identifier, slave
        //        (byte)code, //Function Code
        //        addresses[1], addresses[0], //Starting Address
        //        dataLengths[1], dataLengths[0], //length
        //        (byte)bytes.Length, //byte count
        //    ];
        //    sendBuff = [.. sendBuff, .. bytes];

        //    return sendBuff;
        //}
    }
}
