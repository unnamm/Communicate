using Builder.Interface;
using Builder.Packet;
using Com.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder
{
    /// <summary>
    /// communicate device
    /// </summary>
    public class Device : IProtocol, IDisposable
    {
        private readonly Communicate _com;

        public Device(Communicate com)
        {
            _com = com;
        }

        public Task ConnectAsync()
        {
            if (_com.IsConnected)
            {
                return Task.CompletedTask;
            }
            return _com.ConnectAsync();
        }

        public void Dispose() => _com.Dispose();

        public async Task<T> QueryAsync<T>(QueryPacket<T> packet, params object[] @params)
        {
            packet.Params = @params;
            var sendData = MakeParamsCommand(packet.GetCommand(), @params);

            var receiveData = await _com.QueryAsync(Encoding.UTF8.GetBytes(sendData));

            return packet.GetData(Encoding.UTF8.GetString(receiveData));
        }

        public async Task WriteAsync(WritePacket packet, params object[] @params)
        {
            packet.Params = @params;
            var sendData = MakeParamsCommand(packet.GetCommand(), @params);

            await _com.WriteAsync(Encoding.UTF8.GetBytes(sendData));
        }

        /// <summary>
        /// command + params
        /// </summary>
        /// <param name="command"></param>
        /// <param name="writeParams"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string MakeParamsCommand(string command, object[]? writeParams)
        {
            if (command.Contains("{0}"))
            {
                if (writeParams == null || writeParams.Length == 0)
                {
                    throw new Exception("param need");
                }

                return string.Format(command, writeParams);
            }
            //else
            //{
            //    if (writeParams != null && writeParams.Length > 0)
            //    {
            //        throw new Exception("param must empty");
            //    }
            //}

            return command;
        }
    }
}
