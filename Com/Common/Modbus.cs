using Com.Interface;
using System.Collections;

namespace Com.Common
{
    public abstract class Modbus
    {
        protected readonly bool _isUseCRC;
        protected readonly ICommunicate _communicate;

        public Modbus(ICommunicate c, bool isUseCRC)
        {
            _communicate = c;
            _isUseCRC = isUseCRC;
        }

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
    }
}
