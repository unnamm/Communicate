using Com.Interface;
using System.Collections;

namespace Com.Common
{
    public abstract class Modbus
    {
        protected readonly ICommunicate _communicate;

        public Modbus(ICommunicate c)
        {
            _communicate = c;
        }

        /// <summary>
        /// ushort -> bool[16]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BitArray GetBitArrayFromUshort(ushort value) => new(BitConverter.GetBytes(value));

        /// <summary>
        /// bool[16] -> ushort
        /// </summary>
        /// <param name="bitArray"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ushort GetushortFromBitArray(BitArray bitArray)
        {
            if (bitArray.Count != sizeof(ushort) * 8) //need 16 bytes
            {
                throw new Exception("bit array size error");
            }

            byte[] bytes = new byte[2];
            bitArray.CopyTo(bytes, 0);

            return BitConverter.ToUInt16(bytes);
        }

        public static float ConvertFloatFromUshorts(ushort[] values) //ushort array -> float
        {
            if (values.Length != 2)
                throw new Exception("values count need two");

            var final = Array.ConvertAll([values[0], values[1]], BitConverter.GetBytes);
            return BitConverter.ToSingle([final[0][0], final[0][1], final[1][0], final[1][1]], 0);
        }

        public static ushort[] ConvertUshortsFromFloat(float value) //float -> ushort array
        {
            var bytes = BitConverter.GetBytes(value);
            return [BitConverter.ToUInt16(bytes, 0), BitConverter.ToUInt16(bytes, 2)];
        }
    }
}
