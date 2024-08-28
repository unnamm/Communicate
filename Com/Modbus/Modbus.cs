using Com.Common;
using System.Collections;

namespace Com.Modbus
{
    /// <summary>
    /// modbus function
    /// </summary>
    public abstract class Modbus
    {
        readonly protected Communicate _communicate;

        protected ushort _transactionId = 0;

        public Modbus(Communicate c)
        {
            if (c.IsConnectStream() == false)
                throw new Exception("communicate disconnected");

            _communicate = c;
        }

        /// <summary>
        /// query and check transactionId
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected async Task<byte[]> query(byte[] data)
        {
            var read = await _communicate.QueryAsync(data);

            var transaction = BitConverter.ToUInt16([read[1], read[0]]);

            if (_transactionId != transaction)
            {
                throw new Exception("pair error");
            }

            _transactionId = _transactionId == ushort.MaxValue ? (ushort)0 : (ushort)(_transactionId + 1);

            return read;
        }

        public static BitArray GetBitArray(ushort value) =>
            new(BitConverter.GetBytes(value).ToArray());

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
    }
}
