using Common;
using System.Net.Sockets;

namespace Tcp
{
    /// <summary>
    /// tcp client
    /// </summary>
    public class TcpCommunicate : Communicate
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly TcpClient _client = new();

        public TcpCommunicate(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        protected override async Task<Stream> connectAsync()
        {
            await _client.ConnectAsync(_ip, _port);
            return _client.GetStream();
        }

        public override void Dispose()
        {
            base.Dispose();
            _client.Close();
        }
    }
}
