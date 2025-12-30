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
    public abstract class Communicate : IQueryProtocol
    {
        private readonly int _timeoutMilli;
        private readonly SemaphoreSlim _slimQuery = new(1); //other thread lock

        private Stream _stream;

        public Communicate(int timeout)
        {
            _timeoutMilli = timeout;
        }

        public async Task ConnectAsync()
        {
            _stream = await ConnectAndStream(_timeoutMilli);

            if (_stream is null)
                throw new NullReferenceException("stream is null");
        }

        public async Task<byte[]> QueryAsync(IEnumerable<byte> data, int readDelay = 0)
        {
            byte[] receive;

            await _slimQuery.WaitAsync();
            try
            {
                using CancellationTokenSource cts = new(_timeoutMilli);
                await _stream.WriteAsync(data.ToArray(), cts.Token);
                await Task.Delay(readDelay);
                receive = await ReadAsync();
            }
            catch (Exception ex) //release slim lock and throw
            {
                _slimQuery.Release();
                throw new Exception(string.Empty, ex);
            }
            _slimQuery.Release();

            return receive;
        }

        public ValueTask WriteAsync(byte[] data, int timeoutMilli)
        {
            using CancellationTokenSource cts = new(timeoutMilli);
            return _stream.WriteAsync(data, cts.Token);
        }

        public ValueTask WriteAsync(byte[] data) => WriteAsync(data, _timeoutMilli);

        private async Task<byte[]> Read(int size, int timeoutMilli)
        {
            var buffer = new byte[size];
            using CancellationTokenSource cts = new(timeoutMilli);

            var len = await _stream.ReadAsync(buffer, cts.Token);

            var bufferData = new byte[len];
            Array.Copy(buffer, bufferData, len);
            return bufferData;
        }

        public async Task<byte[]> ReadAsync()
        {
            IEnumerable<byte> resultData = []; //return data

            const int BUFFER_SIZE = 1024;

            var data = await Read(BUFFER_SIZE, _timeoutMilli);

            if (data.Length < BUFFER_SIZE)
                return data;

            resultData = resultData.Concat(data);

            while (true) //receive data >= buffer size
            {
                try
                {
                    data = await Read(BUFFER_SIZE, _timeoutMilli / 10); //read next data
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

        public virtual void Dispose()
        {
            _stream.Dispose();
            GC.SuppressFinalize(this);
        }

        protected abstract Task<Stream> ConnectAndStream(int timeoutMilli);
    }
}
