using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.Modbus
{
    /// <summary>
    /// modbus function
    /// </summary>
    public class Modbus
    {
        public static BitArray GetBitArray(ushort value)
        {
            var byteArray = BitConverter.GetBytes(value).Reverse().ToArray();
            var bitArray = new BitArray(byteArray);
            return bitArray;
        }

        public static ushort Getushort(BitArray bitArray)
        {
            byte[] bytes = new byte[2];
            bitArray.CopyTo(bytes, 0);
            bytes = bytes.Reverse().ToArray();
            return BitConverter.ToUInt16(bytes);
        }
    }
}
