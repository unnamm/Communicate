using Com.Common;
using System.Collections;

namespace Com.Modbus
{
    public abstract class Modbus
    {
        readonly private Communicate _communicate;

        public Modbus(Communicate c)
        {
            _communicate = c;
        }

        /// <summary>
        /// write and read to stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Task<byte[]> Query(byte[] data) => _communicate.QueryAsync(data);

        /// <summary>
        /// write to stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Task WriteAsync(byte[] data) => _communicate.WriteAsync(data).AsTask();

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
            if (bitArray.Count != sizeof(ushort) * 8) //need 16 bytes
            {
                throw new Exception("bit array size error");
            }

            byte[] bytes = new byte[2];
            bitArray.CopyTo(bytes, 0);

            return BitConverter.ToUInt16(bytes.ToArray());
        }

        public abstract Task<ushort[]> ReadRegisters(FunctionCode code, ushort address, ushort readNum, byte slave = 0x01);
        public abstract Task WriteSingleRegister(ushort address, ushort value, byte slave = 0x01);
    }
}
