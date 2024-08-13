using Com.Common;
using System.Net.Sockets;

namespace Com.Tcp
{
    /// <summary>
    /// tcp client
    /// </summary>
    public class TcpCommunicate : Communicate
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly TcpClient _client = new();

        public TcpCommunicate(string ip, int port, int timeout = 1000, int streamEndTimeout = 100) :
            base(timeout, streamEndTimeout)
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
