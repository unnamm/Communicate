﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC.MCProtocol
{
    //public class PLCData
    //{
    //    public static IPlc PLC;
    //}

    class PLCData<T> //: PLCData
    {
        readonly IPlc PLC = new McProtocolTcp();
        readonly PlcDeviceType DeviceType;
        int Address;
        int Length;
        int LENGTH;//Length in bytes
        byte[] bytes;
        public PLCData(PlcDeviceType DeviceType, int Address, int Length)
        {
            this.DeviceType = DeviceType;
            this.Address = Address;

            string t = typeof(T).Name;
            switch (t)
            {
                case "Boolean":
                    this.LENGTH = (Length / 16 + (Length % 16 > 0 ? 1 : 0)) * 2;
                    this.Length = Length;
                    break;
                case "Int32":
                    this.LENGTH = 4 * Length;
                    this.Length = Length * 2;
                    break;
                case "Int16":
                    this.LENGTH = 2 * Length;
                    this.Length = Length;
                    break;
                case "UInt16":
                    this.LENGTH = 2 * Length;
                    this.Length = Length;
                    break;
                case "UInt32":
                    this.LENGTH = 4 * Length;
                    this.Length = Length * 2;
                    break;
                case "Single":
                    this.LENGTH = 4 * Length;
                    this.Length = Length * 2;
                    break;
                case "Double":
                    this.LENGTH = 8 * Length;
                    this.Length = Length * 4;
                    break;
                case "Char":
                    this.LENGTH = Length;
                    this.Length = Length;
                    break;
                default:
                    throw new Exception("Type not supported by PLC.");
            }
            this.bytes = new byte[this.LENGTH];

        }
        public T this[int i]
        {
            get
            {
                Union u = new Union();
                string t = typeof(T).Name;
                switch (t)
                {
                    case "Boolean":
                        return (T)Convert.ChangeType(((this.bytes[i / 8] >> (i % 8)) % 2 == 1), typeof(T));
                    case "Int32":
                        u.a = this.bytes[i * 4];
                        u.b = this.bytes[i * 4 + 1];
                        u.c = this.bytes[i * 4 + 2];
                        u.d = this.bytes[i * 4 + 3];
                        return (T)Convert.ChangeType(u.DINT, typeof(T));
                    case "Int16":
                        u.a = this.bytes[i * 2];
                        u.b = this.bytes[i * 2 + 1];
                        return (T)Convert.ChangeType(u.INT, typeof(T));
                    case "UInt16":
                        u.a = this.bytes[i * 2];
                        u.b = this.bytes[i * 2 + 1];
                        return (T)Convert.ChangeType(u.UINT, typeof(T));
                    case "UInt32":
                        u.a = this.bytes[i * 4];
                        u.b = this.bytes[i * 4 + 1];
                        u.c = this.bytes[i * 4 + 2];
                        u.d = this.bytes[i * 4 + 3];
                        return (T)Convert.ChangeType(u.UDINT, typeof(T));
                    case "Single":
                        u.a = this.bytes[i * 4];
                        u.b = this.bytes[i * 4 + 1];
                        u.c = this.bytes[i * 4 + 2];
                        u.d = this.bytes[i * 4 + 3];
                        return (T)Convert.ChangeType(u.REAL, typeof(T));
                    case "Char":
                        return (T)Convert.ChangeType(this.ToString()[i], typeof(T));
                    default:
                        throw new Exception("Type not recognized.");
                }
            }
            set
            {
                Union u = new Union();
                string t = typeof(T).Name;
                switch (t)
                {
                    case "Boolean":
                        bool arg = Convert.ToBoolean(value);
                        if (arg && (this.bytes[i / 8] >> (i % 8)) % 2 == 0)
                            this.bytes[i / 8] += (byte)(1 << (i % 8));
                        else if (!arg && (this.bytes[i / 8] >> (i % 8)) % 2 == 1)
                            this.bytes[i / 8] -= (byte)(1 << (i % 8));
                        return;
                    case "Int32":
                        u.DINT = Convert.ToInt32(value);
                        this.bytes[i * 4] = u.a;
                        this.bytes[i * 4 + 1] = u.b;
                        this.bytes[i * 4 + 2] = u.c;
                        this.bytes[i * 4 + 3] = u.d;
                        return;
                    case "Int16":
                        u.INT = Convert.ToInt16(value);
                        this.bytes[i * 2] = u.a;
                        this.bytes[i * 2 + 1] = u.b;
                        return;
                    case "UInt32":
                        u.UDINT = Convert.ToUInt32(value);
                        this.bytes[i * 4] = u.a;
                        this.bytes[i * 4 + 1] = u.b;
                        this.bytes[i * 4 + 2] = u.c;
                        this.bytes[i * 4 + 3] = u.d;
                        return;
                    case "UInt16":
                        u.UINT = Convert.ToUInt16(value);
                        this.bytes[i * 2] = u.a;
                        this.bytes[i * 2] = u.b;
                        return;
                    case "Single":
                        u.REAL = Convert.ToSingle(value);
                        this.bytes[i * 4] = u.a;
                        this.bytes[i * 4 + 1] = u.b;
                        this.bytes[i * 4 + 2] = u.c;
                        this.bytes[i * 4 + 3] = u.d;
                        return;
                    default:
                        throw new Exception("Type not recognized.");
                }
            }
        }

        public async Task WriteData()
        {
            await PLC.WriteDeviceBlock(this.DeviceType, this.Address, Length, bytes);
        }
        public async Task ReadData()
        {
            this.bytes = await PLC.ReadDeviceBlock(DeviceType, this.Address, this.Length);
        }

    }
}
