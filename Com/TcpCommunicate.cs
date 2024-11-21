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

        public TcpCommunicate(string ip, int port, int timeout = 1000) :
            base(timeout)
        {
            _ip = ip;
            _port = port;
        }

        protected override async Task<Stream> GetStreamAfterConnect()
        {
            await _client.ConnectAsync(_ip, _port).Timeout(_timeout);
            return _client.GetStream();
        }

        public override void Dispose()
        {
            base.Dispose();
            _client.Dispose();
        }

        public static async void TestPlay()
        {
            TcpCommunicate tcp = new("127.0.0.1", 6053, 5000);
            await tcp.ConnectAsync();
            var v = await tcp.ReadAsync();
            Console.WriteLine(Encoding.UTF8.GetString(v));
        }
    }
}
