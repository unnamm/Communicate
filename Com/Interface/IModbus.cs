using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Interface
{
    /*
    There are many types of devices

    first. one address : one 1bit
    this device is just bit
    but, many devices use 16bit word

    second. one address : one 16bit word
    16bit word can to get 16 nums coils, one ushort -> 16 bool state
    ex) 7600 = 0x0001 -> 7600.0 = true, 7600.1 = false ... 7600.15 = false

    third. two address : one 32bit datatype
    address, address+1 => float or int
    */

    public interface IModbus
    {
        const int SLAVE_FIRST = 0x01;

        #region one address : one bit

        /// <summary>
        /// func code: 0x01,
        /// read setting bit
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="readNum"></param>
        /// <param name="slave"></param>
        /// <returns>key: address, value: bool</returns>
        Task<IEnumerable<bool[]>> ReadCoils(ushort startAddress, ushort readNum, byte slave = SLAVE_FIRST);

        /// <summary>
        /// func code: 0x05,
        /// write setting bit
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="slave"></param>
        void WriteSingleCoil(ushort address, bool value, byte slave = SLAVE_FIRST);

        /// <summary>
        /// func code: 0x15,
        /// write setting multiple bits
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="values"></param>
        /// <param name="slave"></param>
        void WriteMultipleCoils(ushort startAddress, IEnumerable<bool> values, byte slave = SLAVE_FIRST);

        /// <summary>
        /// func code: 0x02,
        /// only read current bit state
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="readNum"></param>
        /// <param name="slave"></param>
        /// <returns>key: address, value: bool</returns>
        Task<IEnumerable<bool[]>> ReadDiscreteInputs(ushort startAddress, ushort readNum, byte slave = SLAVE_FIRST);

        #endregion

        #region one address : one word


        /// <summary>
        /// func code: 0x03,
        /// read setting word
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="readNum"></param>
        /// <param name="slave"></param>
        /// <returns>key: address, value: ushort</returns>
        Task<IEnumerable<ushort>> ReadHoldingRegisters(ushort startAddress, ushort readNum, byte slave = SLAVE_FIRST);

        /// <summary>
        /// func code: 0x06,
        /// write setting word
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <param name="slave"></param>
        void WriteSingleRegister(ushort address, ushort data, byte slave = SLAVE_FIRST);

        /// <summary>
        /// func code: 0x16,
        /// write setting multiple words
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <param name="slave"></param>
        void WriteMultipleRegisters(ushort address, IEnumerable<ushort> data, byte slave = SLAVE_FIRST);

        /// <summary>
        /// func code: 0x04,
        /// only read current word state
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="readNum"></param>
        /// <param name="slave"></param>
        /// <returns>key: address, value: ushort</returns>
        Task<IEnumerable<ushort>> ReadInputRegisters(ushort startAddress, ushort readNum, byte slave = SLAVE_FIRST);

        #endregion
    }
}
