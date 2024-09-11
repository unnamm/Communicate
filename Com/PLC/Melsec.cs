using Com.Common;

namespace Com.PLC
{
    /// <summary>
    /// need ActUtlTypeLib.dll
    /// </summary>
    public class Melsec
    {
        private readonly ActUtlTypeLib.ActUtlType _plc = new();

        public void Connect(int actLogicalStationNumber)
        {
            _plc.ActLogicalStationNumber = actLogicalStationNumber;

            if (_plc.Open() != 0)
            {
                throw new Exception("connect fail");
            }
        }

        public void Write(string address, int value)
        {
            _plc.SetDevice(address, value);
        }
        public int Read(string address)
        {
            _plc.GetDevice(address, out int value);

            return value;
        }
    }
}
