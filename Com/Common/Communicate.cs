using Com.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Common
{
    /// <summary>
    /// communication parent
    /// </summary>
    public abstract class Communicate : IDisposable, IModbusProtocol
    {
        private readonly SemaphoreSlim _slim = new(1); //other thread lock

        protected readonly int _timeout;

        private Stream _stream;

        public abstract bool IsConnected { get; }

        public Communicate(int timeout = 1000) => _timeout = timeout;

        public async Task ConnectAsync()
        {
            _stream = await ConnectAndStream().Timeout(_timeout);

            if (_stream is null)
                throw new Exception("stream is null");
        }

        private async Task<byte[]> Read(int size, int timeout)
        {
            var buffer = new byte[size];

            var len = await _stream.ReadAsync(buffer).Timeout(timeout);

            var bufferData = new byte[len];
            Array.Copy(buffer, bufferData, len);
            return bufferData;
        }

        /// <summary>
        /// read all data
        /// </summary>
        /// <returns>receive bytes</returns>
        public async Task<byte[]> ReadAsync()
        {
            IEnumerable<byte> resultData = []; //return data

            const int BUFFER_SIZE = 1024;

            var data = await Read(BUFFER_SIZE, _timeout);

            if (data.Length < BUFFER_SIZE)
                return data;

            resultData = resultData.Concat(data);

            while (true) //receive data >= buffer size
            {
                try
                {
                    data = await Read(BUFFER_SIZE, _timeout / 10); //read next data
                }
                catch (TimeoutException) //read data empty
                {
                    break;
                }

                resultData = resultData.Concat(data);

                if (data.Length < BUFFER_SIZE) //all data receive
                    break;
            }

            return resultData.ToArray();
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="data">send bytes</param>
        /// <returns>task</returns>
        public ValueTask WriteAsync(IEnumerable<byte> data) => _stream.WriteAsync(data.ToArray()).Timeout(_timeout);

        /// <summary>
        /// write and read
        /// </summary>
        /// <param name="data"></param>
        /// <param name="readDelay">write and delay and read</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> QueryAsync(IEnumerable<byte> data, int readDelay = 0)
        {
            byte[] receive;

            await _slim.WaitAsync();
            try
            {
                await _stream.WriteAsync(data.ToArray()).Timeout(_timeout);
                await Task.Delay(readDelay);
                receive = await ReadAsync();
            }
            catch (Exception ex)
            {
                _slim.Release();
                throw new Exception(string.Empty, ex);
            }
            _slim.Release();

            return receive;
        }

        /// <summary>
        /// module init and set stream
        /// </summary>
        /// <returns></returns>
        protected abstract Task<Stream> ConnectAndStream();

        public virtual void Dispose()
        {
            _stream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
