using Com.Interface;
using Com.Packet;
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
    public abstract class Communicate : IDisposable, IQueryProtocol
    {
        private readonly SemaphoreSlim _slim = new(1); //other thread lock

        protected readonly int _timeout;

        private Stream _stream;

        public Communicate(int timeout = 1000)
        {
            _timeout = timeout;
        }

        public async Task ConnectAsync()
        {
            _stream = await ConnectAndStream().Timeout(_timeout);

            if (_stream is null)
                throw new Exception("stream is null");
        }

        public async Task<T> QueryAsync<T>(QueryPacket<T> packet, params object[] @params)
        {
            packet.Params = @params;
            var sendData = MakeParamsCommand(packet.GetCommand(), @params);

            var receiveData = await QueryAsync(Encoding.UTF8.GetBytes(sendData));

            return packet.GetData(Encoding.UTF8.GetString(receiveData));
        }

        public ValueTask WriteAsync(WritePacket packet, params object[] @params)
        {
            packet.Params = @params;
            var sendData = MakeParamsCommand(packet.GetCommand(), @params);

            return WriteAsync(Encoding.UTF8.GetBytes(sendData));
        }

        private static string MakeParamsCommand(string command, object[]? writeParams)
        {
            if (command.Contains("{0}")) //use param
            {
                if (writeParams == null || writeParams.Length == 0)
                {
                    throw new Exception("param need");
                }

                return string.Format(command, writeParams);
            }
            else //param empty
            {
                if (writeParams != null && writeParams.Length > 0)
                {
                    throw new Exception("param must empty");
                }
            }

            return command;
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
        /// read bigger data then buffer size
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

        public ValueTask WriteAsync(byte[] data)
        {
            return _stream.WriteAsync(data).Timeout(_timeout);
        }

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
            catch (Exception ex) //release slim lock and throw
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
