using Com.Common;
using System.Net.Sockets;

namespace Com
{
    public class TcpCommunicate : Communicate
    {
        private readonly int _port;
        private readonly string _ip;
        private readonly TcpClient _client = new();

        public TcpCommunicate(string ip, int port, int timeout = 1000) : base(timeout)
        {
            _ip = ip;
            _port = port;
        }

        protected override async Task<Stream> ConnectAndStream(int timeoutMilli)
        {
            using CancellationTokenSource cts = new(timeoutMilli);
            await _client.ConnectAsync(_ip, _port, cts.Token);
            return _client.GetStream();
        }

        public override void Dispose()
        {
            base.Dispose();
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
