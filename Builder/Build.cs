using Builder.Interface;
using Com.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder
{
    /// <summary>
    /// one device match packets
    /// </summary>
    public class Build
    {
        private readonly IProtocol _protocol;
        private readonly List<IPacket> _packetList = []; //added all packets

        public Build(IProtocol protocol)
        {
            _protocol = protocol;
        }

        public async Task Query()
        {
            foreach (var packet in _packetList)
            {
                if (packet is IQueryPacket item)
                {
                    var command = MakeParamsCommand(item.QueryCommand(), item.QueryParams);
                    item.ReceiveData = await _protocol.QueryAsync(command);
                }
            }
        }

        public async Task Write()
        {
            foreach (var packet in _packetList)
            {
                if (packet is IWritePacket item)
                {
                    var command = MakeParamsCommand(item.WriteCommand(), item.WriteParams);
                    await _protocol.WriteAsync(command);
                }
            }
        }

        /// <summary>
        /// add packet
        /// </summary>
        /// <param name="packet"></param>
        /// <exception cref="Exception"></exception>
        public void Add(IPacket packet)
        {
            if (_packetList.Any(x => x.GetType() == packet.GetType()))
            {
                throw new Exception("one build one type packet");
            }

            _packetList.Add(packet);
        }

        public T GetPacket<T>() => (T)_packetList.First(x => typeof(T) == x.GetType());

        /// <summary>
        /// WriteCommand + parameters
        /// </summary>
        /// <param name="command"></param>
        /// <param name="writeParams"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string MakeParamsCommand(string command, object[]? writeParams)
        {
            if (command.Contains("{0}"))
            {
                if (writeParams == null || writeParams.Length == 0)
                {
                    throw new Exception("param need");
                }

                return string.Format(command, writeParams);
            }

            return command;
        }
    }
}
