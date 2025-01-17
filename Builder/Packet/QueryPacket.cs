using Builder.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Packet
{
    /// <summary>
    /// write and recieve packet
    /// </summary>
    /// <typeparam name="T">receive data type</typeparam>
    public abstract class QueryPacket<T> : IPacket
    {
        public object[]? Params { get; set; }
        public abstract string GetCommand();

        /// <summary>
        /// get converted data
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public T GetData(string receiveData)
        {
            if (string.IsNullOrEmpty(receiveData))
                throw new NullReferenceException("receive data is empty");

            return Convert(receiveData);
        }

        /// <summary>
        /// string -> T convert
        /// </summary>
        /// <param name="receiveData"></param>
        /// <returns></returns>
        protected abstract T Convert(string receiveData);
    }
}
