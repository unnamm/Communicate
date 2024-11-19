using Com.Common;
using System.Collections;

namespace Protocol.Modbus
{
    public abstract class Modbus
    {
        readonly private Communicate _communicate;

        public Modbus(Communicate c)
        {
            _communicate = c;
        }

        /// <summary>
        /// communication, write and read from stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Task<IEnumerable<byte>> query(byte[] data) => _communicate.QueryAsync(data);

        /// <summary>
        /// ushort -> bool[16]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BitArray GetBitArray(ushort value) => new(BitConverter.GetBytes(value).ToArray());

        /// <summary>
        /// bool[16] -> ushort
        /// </summary>
        /// <param name="bitArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ushort Getushort(BitArray bitArray)
        {
            if (bitArray.Count != sizeof(ushort) * 8)
            {
                throw new Exception("bit array size error");
            }

            byte[] bytes = new byte[2];
            bitArray.CopyTo(bytes, 0);

            return BitConverter.ToUInt16(bytes.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">ReadHolingRegisters, ReadInputRegisters</param>
        /// <param name="address"></param>
        /// <param name="readNum"></param>
        /// <param name="slave"></param>
        /// <returns></returns>
        public abstract Task<ushort[]> ReadRegisters(FunctionCode code, ushort address, ushort readNum, byte slave = 0x01);
        public abstract Task WriteSingleRegister(ushort address, ushort value, byte slave = 0x01);
    }
}
