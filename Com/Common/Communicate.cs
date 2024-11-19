using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Common
{
    /// <summary>
    /// communication parent
    /// </summary>
    public abstract class Communicate
    {
        private readonly SemaphoreSlim _slim = new(1);
        protected readonly int _timeout;

        private Stream _stream;

        public Communicate(int timeout = 1000) => _timeout = timeout;

        public async Task ConnectAsync()
        {
            _stream = await Connect().Timeout(_timeout);

            if (_stream is null)
                throw new Exception();
        }

        public async Task<IEnumerable<byte>> ReadAsync()
        {
            IEnumerable<byte> receive;

            _slim.Wait();
            try
            {
                receive = await Read();
            }
            catch (Exception ex)
            {
                _slim.Release();
                throw new Exception(string.Empty, ex);
            }
            _slim.Release();

            return receive;
        }

        public async Task WriteAsync(IEnumerable<byte> data)
        {
            _slim.Wait();
            try
            {
                await _stream.WriteAsync(data.ToArray()).Timeout(_timeout);
            }
            catch (Exception ex)
            {
                _slim.Release();
                throw new Exception(string.Empty, ex);
            }
            _slim.Release();
        }

        /// <summary>
        /// write and read
        /// </summary>
        /// <param name="data"></param>
        /// <param name="readDelay">write and delay and read</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<byte>> QueryAsync(IEnumerable<byte> data, int readDelay = 0)
        {
            IEnumerable<byte> receive;

            _slim.Wait();
            try
            {
                await _stream.WriteAsync(data.ToArray()).Timeout(_timeout);
                await Task.Delay(readDelay);
                receive = await Read();
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
        /// can bigdata read
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<IEnumerable<byte>> Read()
        {
            if (_stream is null)
                throw new Exception();

            IEnumerable<byte> receiveList = []; //return data

            bool isLoop = false; //check buffer size < read data length

            const int REPEAT_TIMEOUT = 100;

            while (true)
            {
                const int BUFFER_SIZE = 1024;
                int len;

                var buffer = new byte[BUFFER_SIZE];

                if (isLoop)
                {
                    try //repeat read
                    {
                        len = await _stream.ReadAsync(buffer).Timeout(REPEAT_TIMEOUT); //read repeat, wait short time
                    }
                    catch //if len == BUFFER_SIZE, repeat data zero
                    {
                        break;
                    }
                }
                else //first read
                {
                    len = await _stream.ReadAsync(buffer).Timeout(_timeout);
                }

                var receive = new byte[len];
                Array.Copy(buffer, receive, len); //receive data length is receive length
                receiveList = receiveList.Concat(receive);

                if (len < BUFFER_SIZE) //data end
                    break;

                isLoop = true;
            }

            return receiveList;
        }

        /// <summary>
        /// module init and set stream
        /// </summary>
        /// <returns></returns>
        protected abstract Task<Stream> Connect();

        public virtual void Dispose() => _stream.Dispose();

    }
}
