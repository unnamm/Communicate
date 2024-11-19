#define old //Now that .NET Standard is supported in UWP old code is Good

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PLC.MCProtocol
{
    public class McProtocolTcp : McProtocolApp
    {
        // ====================================================================================
        // コンストラクタ
        public override bool Connected
        {
            get
            {
                return Client.Connected;
            }
        }
        public McProtocolTcp() : this("", 0, McFrame.MC3E) { }
        public McProtocolTcp(string iHostName, int iPortNumber, McFrame frame)
            : base(iHostName, iPortNumber, frame)
        {
            CommandFrame = frame;
#if !old
            this.Host = new HostName(iHostName);

            this.streamSocket = new StreamSocket();

            this.Port = iPortNumber;
#endif
            Client = new TcpClient();
        }

        // &&&&& protected &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        override protected Task<int> DoConnect()
        {
#if !old
            this.streamSocket.Control.KeepAlive = true;

            await this.streamSocket.ConnectAsync(this.Host, "" + this.Port);
#endif
#if old
                //TcpClient c = Client;
                if (!Client.Connected)
                {
                    // Keep Alive機能の実装
                    var ka = new List<byte>(sizeof(uint) * 3);
                    ka.AddRange(BitConverter.GetBytes(1u));
                    ka.AddRange(BitConverter.GetBytes(45000u));
                    ka.AddRange(BitConverter.GetBytes(5000u));
                    Client.Client.IOControl(IOControlCode.KeepAliveValues, ka.ToArray(), null);
                    Client.Connect(HostName, PortNumber);
                    Stream = Client.GetStream();

                }
#endif
            return Task.FromResult(0);
        }
        // ====================================================================================
        override protected void DoDisconnect()
        {
#if !old
            this.streamSocket.Dispose();
#endif
#if old
                TcpClient c = Client;
                if (c.Connected)
                {
                    c.Close();
                }
#endif
        }
        // ================================================================================
        async override protected Task<byte[]> Execute(byte[] iCommand)
        {
            List<byte> list = new List<byte>();
#if windows

                //Write to the buffer


                await this.streamSocket.CancelIOAsync();

                Windows.Storage.Streams.DataWriter writer = new Windows.Storage.Streams.DataWriter(this.streamSocket.OutputStream);
                writer.WriteBytes(iCommand);
                await writer.StoreAsync();
                writer.DetachStream();

                
                //Read back from the buffer

                Windows.Storage.Streams.DataReader reader = new Windows.Storage.Streams.DataReader(this.streamSocket.InputStream);
                reader.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
                //Load 4 bytes off the buffer as the header
                //DONE Fix the read length
                //do
                //{
                await reader.LoadAsync(256);
                //if (reader.UnconsumedBufferLength == 0) break;
                byte[] bytes = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(bytes);//Should be 4
                list.AddRange(bytes);
                reader.DetachStream();
                //} while (true);
                return list.ToArray();
#endif

#if old

#endif
#if reading
                    await reader.LoadAsync(header[header.Length - 1]);//The last byte should be the remaining lenght
                    //
                    byte[] data = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(data);
                    byte[] buffer = new byte[header.Length + data.Length];
                    header.CopyTo(buffer, 0);
                    data.CopyTo(buffer, header.Length);
                    
#else


#endif



#if old

                NetworkStream ns = Stream;
                ns.Write(iCommand, 0, iCommand.Length);
                ns.Flush();

                using (var ms = new MemoryStream())
                {
                    var buff = new byte[256];
                    do
                    {
                        int sz = ns.Read(buff, 0, buff.Length);
                        if (sz == 0)
                        {
                            throw new Exception("切断されました");
                        }
                        ms.Write(buff, 0, sz);
                    } while (ns.DataAvailable);
                    return ms.ToArray();
                }
#endif

        }
        // &&&&& private &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
#if !old
        private HostName Host { get; set; }
        private StreamSocket streamSocket { get; set; }
        private int Port { get; set; }
#endif
#if old
            private TcpClient Client { get; set; }
            private NetworkStream Stream { get; set; }
#endif
    }
}
