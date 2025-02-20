using Com.Common;
using System.Net.Sockets;
using System.Text;

namespace Com
{
    public class TcpCommunicate : Communicate
    {
        private readonly int _port;
        private readonly string _ip;
        private readonly TcpClient _client = new();

        //public override bool IsConnected => _client.Connected;

        public TcpCommunicate(string ip, int port, int timeout = 1000) :
            base(timeout)
        {
            _ip = ip;
            _port = port;
        }

        protected override async Task<Stream> ConnectAndStream()
        {
            await _client.ConnectAsync(_ip, _port).Timeout(_timeout);
            return _client.GetStream();
        }

        public override void Dispose()
        {
            _client.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
