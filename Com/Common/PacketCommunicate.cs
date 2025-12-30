using Com.Interface;
using Com.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Common
{
    public class PacketCommunicate : IPacketProtocol
    {
        private readonly Communicate _com;

        public PacketCommunicate(Communicate com)
        {
            _com = com;
        }

        public Task ConnectAsync() => _com.ConnectAsync();

        public void Dispose() => _com.Dispose();

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

        public async Task<T> QueryAsync<T>(QueryPacket<T> packet, params object[] @params)
        {
            var sendData = MakeParamsCommand(packet.GetCommand(), @params);
            var receiveData = await _com.QueryAsync(Encoding.UTF8.GetBytes(sendData));
            return packet.GetData(Encoding.UTF8.GetString(receiveData));
        }

        public ValueTask WriteAsync(WritePacket packet, params object[] @params)
        {
            var sendData = MakeParamsCommand(packet.GetCommand(), @params);
            return _com.WriteAsync(Encoding.UTF8.GetBytes(sendData));
        }
    }
}
