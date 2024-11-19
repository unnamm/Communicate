using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC.MCProtocol
{
    // 通信に使用するコマンドを表現するインナークラス
    class McCommand
    {
        public McFrame FrameType { get; private set; }  // フレーム種別
        private uint SerialNumber { get; set; }  // シリアル番号
        private uint NetworkNumber { get; set; } // ネットワーク番号
        private uint PcNumber { get; set; }      // PC番号
        private uint IoNumber { get; set; }      // 要求先ユニットI/O番号
        private uint ChannelNumber { get; set; } // 要求先ユニット局番号
        private uint CpuTimer { get; set; }      // CPU監視タイマ
        private int ResultCode { get; set; }     // 終了コード
        public byte[] Response { get; private set; }    // 応答データ
                                                        // ================================================================================
                                                        // コンストラクタ
        public McCommand(McFrame iFrame)
        {
            FrameType = iFrame;
            SerialNumber = 0x0001u;
            NetworkNumber = 0x0000u;
            PcNumber = 0x00FFu;
            IoNumber = 0x03FFu;
            ChannelNumber = 0x0000u;
            CpuTimer = 0x0010u;
        }
        // ================================================================================
        public byte[] SetCommandMC1E(byte Subheader, byte[] iData)
        {
            List<byte> ret = new List<byte>(iData.Length + 4);
            ret.Add(Subheader);
            ret.Add((byte)this.PcNumber);
            ret.Add((byte)CpuTimer);
            ret.Add((byte)(CpuTimer >> 8));
            ret.AddRange(iData);
            return ret.ToArray();
        }
        public byte[] SetCommandMC3E(uint iMainCommand, uint iSubCommand, byte[] iData)
        {
            var dataLength = (uint)(iData.Length + 6);
            List<byte> ret = new List<byte>(iData.Length + 20);
            uint frame = 0x0050u;
            ret.Add((byte)frame);
            ret.Add((byte)(frame >> 8));

            ret.Add((byte)NetworkNumber);

            ret.Add((byte)PcNumber);

            ret.Add((byte)IoNumber);
            ret.Add((byte)(IoNumber >> 8));
            ret.Add((byte)ChannelNumber);
            ret.Add((byte)dataLength);
            ret.Add((byte)(dataLength >> 8));


            ret.Add((byte)CpuTimer);
            ret.Add((byte)(CpuTimer >> 8));
            ret.Add((byte)iMainCommand);
            ret.Add((byte)(iMainCommand >> 8));
            ret.Add((byte)iSubCommand);
            ret.Add((byte)(iSubCommand >> 8));

            ret.AddRange(iData);
            return ret.ToArray();
        }
        public byte[] SetCommandMC4E(uint iMainCommand, uint iSubCommand, byte[] iData)
        {
            var dataLength = (uint)(iData.Length + 6);
            var ret = new List<byte>(iData.Length + 20);
            uint frame = 0x0054u;
            ret.Add((byte)frame);
            ret.Add((byte)(frame >> 8));
            ret.Add((byte)SerialNumber);
            ret.Add((byte)(SerialNumber >> 8));
            ret.Add(0x00);
            ret.Add(0x00);
            ret.Add((byte)NetworkNumber);
            ret.Add((byte)PcNumber);
            ret.Add((byte)IoNumber);
            ret.Add((byte)(IoNumber >> 8));
            ret.Add((byte)ChannelNumber);
            ret.Add((byte)dataLength);
            ret.Add((byte)(dataLength >> 8));
            ret.Add((byte)CpuTimer);
            ret.Add((byte)(CpuTimer >> 8));
            ret.Add((byte)iMainCommand);
            ret.Add((byte)(iMainCommand >> 8));
            ret.Add((byte)iSubCommand);
            ret.Add((byte)(iSubCommand >> 8));

            ret.AddRange(iData);
            return ret.ToArray();
        }
        // ================================================================================
        public int SetResponse(byte[] iResponse)
        {
            int min;
            switch (FrameType)
            {
                case McFrame.MC1E:
                    min = 2;
                    if (min <= iResponse.Length)
                    {
                        //There is a subheader, end code and data.                                    

                        ResultCode = (int)iResponse[min - 2];
                        Response = new byte[iResponse.Length - 2];
                        Buffer.BlockCopy(iResponse, min, Response, 0, Response.Length);
                    }
                    break;
                case McFrame.MC3E:
                    min = 11;
                    if (min <= iResponse.Length)
                    {
                        var btCount = new[] { iResponse[min - 4], iResponse[min - 3] };
                        var btCode = new[] { iResponse[min - 2], iResponse[min - 1] };
                        int rsCount = BitConverter.ToUInt16(btCount, 0);
                        ResultCode = BitConverter.ToUInt16(btCode, 0);
                        Response = new byte[rsCount - 2];
                        Buffer.BlockCopy(iResponse, min, Response, 0, Response.Length);
                    }
                    break;
                case McFrame.MC4E:
                    min = 15;
                    if (min <= iResponse.Length)
                    {
                        var btCount = new[] { iResponse[min - 4], iResponse[min - 3] };
                        var btCode = new[] { iResponse[min - 2], iResponse[min - 1] };
                        int rsCount = BitConverter.ToUInt16(btCount, 0);
                        ResultCode = BitConverter.ToUInt16(btCode, 0);
                        Response = new byte[rsCount - 2];
                        Buffer.BlockCopy(iResponse, min, Response, 0, Response.Length);
                    }
                    break;
                default:
                    throw new Exception("Frame type not supported.");

            }
            return ResultCode;
        }
        // ================================================================================
        public bool IsIncorrectResponse(byte[] iResponse, int minLenght)
        {
            //TEST add 1E frame
            switch (this.FrameType)
            {
                case McFrame.MC1E:
                    return ((iResponse.Length < minLenght));

                case McFrame.MC3E:
                case McFrame.MC4E:
                    var btCount = new[] { iResponse[minLenght - 4], iResponse[minLenght - 3] };
                    var btCode = new[] { iResponse[minLenght - 2], iResponse[minLenght - 1] };
                    var rsCount = BitConverter.ToUInt16(btCount, 0) - 2;
                    var rsCode = BitConverter.ToUInt16(btCode, 0);
                    return (rsCode == 0 && rsCount != (iResponse.Length - minLenght));

                default:
                    throw new Exception("Type Not supported");

            }
        }
    }
}
