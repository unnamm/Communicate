using System;
using System.Collections.Generic;
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
        protected readonly int _timeout;

        private readonly int _streamEndTimeout;
        private readonly SemaphoreSlim _slim = new(1, 1);

        private Stream? _stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">communication timeout</param>
        /// <param name="streamEndTimeout">repeat read timeout, wait read end</param>
        /// <exception cref="Exception"></exception>
        public Communicate(int timeout = 1000, int streamEndTimeout = 100)
        {
            if (streamEndTimeout >= timeout)
                throw new Exception();

            _timeout = timeout;
            _streamEndTimeout = streamEndTimeout;
        }

        public bool IsConnectStream()
        {
            if (_stream == null)
                return false;
            return _stream.CanRead;
        }

        /// <summary>
        /// release stream
        /// </summary>
        /// <exception cref="Exception"></exception>
        public virtual void Dispose()
        {
            if (_stream is null)
                throw new Exception();

            _stream.Close();
        }

        /// <summary>
        /// initialize stream
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task ConnectAsync()
        {
            _stream = await connectAsync().Timeout(_timeout);

            if (_stream is null)
                throw new Exception();
        }

        /// <summary>
        /// read stream
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> ReadAsync()
        {
            _slim.Wait();
            var receive = await readAsync().Timeout(_timeout);
            _slim.Release();

            return receive;
        }

        /// <summary>
        /// write data
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="Exception"></exception>
        public async void WriteAsync(byte[] data)
        {
            if (_stream is null)
                throw new Exception();

            _slim.Wait();
            await writeAsync(data).Timeout(_timeout);
            _slim.Release();
        }

        /// <summary>
        /// write and read
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> QueryAsync(byte[] data)
        {
            if (_stream is null)
                throw new Exception();

            byte[] receive;
            _slim.Wait();

            try
            {
                await writeAsync(data).Timeout(_timeout);
                receive = await readAsync().Timeout(_timeout);
            }
            catch (Exception ex)
            {
                _slim.Release();
                throw new Exception(ex.Message);
            }

            _slim.Release();

            return receive;
        }

        /// <summary>
        /// module init and set stream
        /// </summary>
        /// <returns></returns>
        protected abstract Task<Stream> connectAsync();

        /// <summary>
        /// read
        /// </summary>
        /// <returns>receive data</returns>
        /// <exception cref="Exception"></exception>
        private async Task<byte[]> readAsync()
        {
            if (_stream is null)
                throw new Exception();

            IEnumerable<byte> receiveList = []; //return data

            bool isLoop = false; //check buffer size < read data length

            while (true)
            {
                const int BUFFER_SIZE = 1024;
                int len;

                var buffer = new byte[BUFFER_SIZE];

                if (isLoop)
                {
                    try
                    {
                        len = await _stream.ReadAsync(buffer).Timeout(_streamEndTimeout); //read repeat, wait short time
                    }
                    catch //if stream read length == BUFFER_SIZE
                    {
                        break;
                    }
                }
                else
                {
                    len = await _stream.ReadAsync(buffer);
                }

                var receive = new byte[len];
                Array.Copy(buffer, receive, len); //receive data length is receive length
                receiveList = receiveList.Concat(receive);

                if (len < BUFFER_SIZE) //data end
                    break;

                isLoop = true;
            }

            return [.. receiveList];
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="data">send data</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task writeAsync(byte[] data)
        {
            if (_stream is null)
                throw new Exception();

            await _stream.WriteAsync(data);
        }
    }
}
