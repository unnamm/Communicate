using Builder.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Packet
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">receive data type</typeparam>
    public abstract class QueryPacket<T> : IQueryPacket
    {
        public object[]? QueryParams { get; set; }

        public string? ReceiveData { get; set; }

        /// <summary>
        /// get converted data
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public T GetData()
        {
            if (string.IsNullOrEmpty(ReceiveData))
                throw new NullReferenceException("query is null or empty");

            return Convert(ReceiveData);
        }

        public abstract string QueryCommand();

        /// <summary>
        /// string -> T convert
        /// </summary>
        /// <param name="receiveData"></param>
        /// <returns></returns>
        protected abstract T Convert(string receiveData);
    }
}
