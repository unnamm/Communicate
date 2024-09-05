using Com.Common;
using System.Collections;

namespace Com.Modbus
{
    /// <summary>
    /// modbus function
    /// </summary>
    public abstract class Modbus
    {
        readonly private Communicate _communicate;

        public Modbus(Communicate c)
        {
            if (c.IsConnectStream() == false)
                throw new Exception("communicate disconnected");

            _communicate = c;
        }

        protected Task<byte[]> query(byte[] data)
        {
            return _communicate.QueryAsync(data);
        }

        public static BitArray GetBitArray(ushort value) => new(BitConverter.GetBytes(value).ToArray());

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
