/********************************************************************************************************************
*********************************************************************************************************************
**
** Description
** -----------
** 1) LS ELECTRIC PLC와 PC의 응용프로그램 간 통신을 위해 XGCommLib.dll을 이용하여 BIT, BYTE, WORD 단위로 읽고 쓰기를    
**    쉽게하기 위한 Wrapper Class
** 2) 신규 프로젝트에서 XGCommLib 모듈을 사용하기 위해서는 XGCommLib.vb파일을 프로젝트에 포함시킴
** 3) 해당 Module에서 Connect 함수를 호출하여 PLC와 연결한 후 필요한 Read/Write 함수를 값을 읽고 쓰기할 수 있음
** 4) PC에서 XGCommLib.dll파일을 이용해서 LS ELECTRIC PLC와 XGT Protocol 통신을 하기 위해서는 윈도우 명령 실행창을 
**    관리자모드로 실행한 후 "regsvr32 C:\XG5000\XGCommLib.dll"을 입력 후 엔터키를 입력함
**    - "xgcommlib.dll에서 DllRegisterServer이(가) 성공했습니다." 메세지 창이 뜨면 정상 설치됨
**    - XG5000을 설치하지 않고 사용시 제공받은 XGCommLib.dll 파일을 반드시 "C:\XG5000" 폴더를 생성한 후 복사하여 
**      위에 설명한 방법과 동일하게 윈도우 명령 실행창에 "regsvr32 C:\XG5000\XGCommLib.dll"을 입력 후 엔터키를 
**      입력하여 XGCommLib.dll을 등록한 후 사용
**
*********************************************************************************************************************
********************************************************************************************************************/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGCommLib;

namespace PLC
{
    public enum XGCOMM_PRE_DEFINES : uint
    {
        MAX_RW_BIT_SIZE = 64,
        MAX_RW_BYTE_SIZE = 1000,
        MAX_RW_WORD_SIZE = 500,
        DEF_PLC_SERVER_TIME_OUT = 15000,
        DEF_PLC_KEEP_ALIVE_TIME = 10000
    }

    public enum DEF_DATA_TYPE : uint
    {
        DATA_TYPE_BIT = 0,
        DATA_TYPE_BYTE = 1,
        DATA_TYPE_WORD = 2
    }

    public enum XGCOMM_FUNC_RESULT : uint
    {
        RT_XGCOMM_SUCCESS = 0,                  // 함수가 수행 성공

        RT_XGCOMM_CAN_NOT_FIND_DLL = 1,         // XGCommLib.dll 파일을 찾을 수 없음, 윈도우 system32폴더의 regsvr32.exe를 이용해 등록필요
        RT_XGCOMM_FAILED_CONNECT = 2,           // PLC와 통신 접속 실패
        RT_XGCOMM_FAILED_KEEPALIVE = 3,         // PLC와 통신 접속 상태 유지 실패

        RT_XGCOMM_INVALID_COMM_DRIVER = 5,      // Comm Driver가 유효하지 않음, Connect함수를 호출하지않았거나 Disconnect를 호출한 상태
        RT_XGCOMM_INVALID_POINT = 6,	        // 함수의 인자로 전달한 배열 포인트가 NULL일 때   

        RT_XGCOMM_FAILED_RESULT = 10,           // XGCommLib.dll의 함수 실행이 실패했을 때
        RT_XGCOMM_FAILED_READ = 11,             // XGCommLib.dll의 ReadRandomDevice 함수의 반환값이 0으로 실패했을 때
        RT_XGCOMM_FAILED_WRITE = 12,            // XGCommLib.dll의 WriteRandomDevice 함수의 반환값이 0으로 실패했을 때

        RT_XGCOMM_ABOVE_MAX_BIT_SIZE = 20,      // Bit 함수의 Bit Size가 32를 초과했을 때(ReadDataBit, WriteDataBit)
        RT_XGCOMM_ABOVE_MAX_BYTE_SIZE = 21,     // Byte 함수의 Byte Size가 260를 초과했을 때(ReadDataByte, WriteDataByte)
        RT_XGCOMM_ABOVE_MAX_WORD_SIZE = 22,     // Word 함수의 Word Size가 130를 초과했을 때(ReadDataWord, WriteDataWord)
        RT_XGCOMM_BLOW_MIN_SIZE = 23,           // Size가 1보다 작을 때

        RT_XGCOMM_FAILED_GET_TIMEOUT = 25,	    // 타임아웃읽기 실패
        RT_XGCOMM_FAILED_SET_TIMEOUT = 26,	    // 타임아웃설정 실패
    }

    internal class XGCommSocket
    {
        private CommObject20? m_CommDriver;
        private int m_nLastCommTime = 0;
        private long m_lPortNo;
        private string m_strIP = string.Empty;

        private readonly object m_MonitorLock = new();

        // -> Comm Driver를 생성하고 지정한 IP의 PLC에 접속함(※ 라이브러리 사용시 초기 한번은 반드시 실행해야됨)
        //  > strIP : 접속할 PLC의 IP를 지정함
        //  > lPort : 접속할 포트 번호를 지정함
        public uint Connect(string strIP, long lPort)
        {
            if ((m_strIP != strIP) || (m_lPortNo != lPort))
            {
                if (this.m_CommDriver != null)
                {
                    this.m_CommDriver.RemoveAll();
                    this.m_CommDriver.Disconnect();
                    this.m_CommDriver = null;
                }

                string strConnection = string.Format("{0}:{1}", strIP, lPort);
                CommObjectFactory20 factory = new CommObjectFactory20();
                this.m_CommDriver = factory.GetMLDPCommObject20(strConnection);
            }
            else
            {
                if (this.m_CommDriver == null)
                {
                    string strConnection = string.Format("{0}:{1}", strIP, lPort);
                    CommObjectFactory20 factory = new CommObjectFactory20();
                    this.m_CommDriver = factory.GetMLDPCommObject20(strConnection);
                }
                else
                {
                    m_CommDriver.Disconnect();
                }
            }

            if (0 == m_CommDriver.Connect(""))
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_CONNECT;
            }

            m_strIP = strIP;
            m_lPortNo = lPort;

            m_nLastCommTime = Environment.TickCount;
            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        public uint Disconnect()
        {
            if (this.m_CommDriver != null)
            {
                this.m_CommDriver.Disconnect();

                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
            }
            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
        }

        // -> 이 함수는 10초간 Read/Write가 없었으면 통신 유지를 위해 Dummy Packet을 송신하여 통신 끊김을 방지함 
        //    ※ XGI, XGK PLC는 XGT통신 서버로 통작할 때 기본 15초 동안 Packet 수신이 없으면 자동으로 Port가 닫힘
        //    Ex) OnTimer나 Thread안에서 1초 간격으로 UpdateKeepAlive 함수를 호출하면 마지막 통신 시간을 체크하여 10초 이상
        //        되면 자동으로 Dummy Packet을 송신함(마지막 통신 시간이 10초 이전이면 아무것도 하지않고 바로 리턴함)
        public uint UpdateKeepAlive()
        {
            uint dwTimeSpen;
            uint dwReturn;

            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            dwTimeSpen = (uint)TICKS_DIFF(m_nLastCommTime, Environment.TickCount);

            if (dwTimeSpen > (uint)XGCOMM_PRE_DEFINES.DEF_PLC_KEEP_ALIVE_TIME)
            {
                dwReturn = ReadDataBit('F', 0, 1, null);
                if (dwReturn != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    if (dwTimeSpen > (uint)XGCOMM_PRE_DEFINES.DEF_PLC_SERVER_TIME_OUT)
                    {
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_KEEPALIVE;
                    }
                }
                else
                {
                    m_nLastCommTime = Environment.TickCount;
                }
            }

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        // -> Connect한 IP와 Port 번호를 확인함
        public uint GetConnectionIP(string pStrIP)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            pStrIP = m_strIP;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        public uint GetConnectionPortNo(ref long lpPortNo)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            lpPortNo = m_lPortNo;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        //////////////////////////////////////////////////////////////////////////
        // -> Bit 단위로 지정한 Offset부터 lSizeBit 갯수만큼 읽기를 수행함, 
        //    ※ Connect 함수가 최초 호출된 이후에는 연결끊김 발생시 함수 내부에서 1회 자동 연결 시도함(연결안 될 경우 110ms정도 후 리턴됨) 
        //  > szDeviceType    : 디바이스 타입을 지정함, Ex) 'M', 'W', 'R', ...
        //  > lOffsetBit      : 읽을 Bit의 offset을 지정함, Ex) MX1000 이면 1000
        //  > lSizeBit        : 한번에 읽을 Bit 개수를 지정함, 최대 32개, Ex) %MX100, %MX101일 읽으려면, lOffsetBit에 100을 지정하고 lSizeBit는 2를 지정
        //  < *pbyRead        : Bit 배열 포인트를 지정, 반드시 배열의 갯수는 lSizeBit 보다 같거나 커야됨 
        public uint ReadDataBit(char szDeviceType, long lOffsetBit, long lSizeBit, byte[]? pbyRead)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            if (lSizeBit > (uint)XGCOMM_PRE_DEFINES.MAX_RW_BIT_SIZE)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_BIT_SIZE;
            }

            if (lSizeBit < 1)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_BLOW_MIN_SIZE;
            }

            uint dwReteurn;
            long lRetValue = 0, lCount = 0, lByteOffset, lBitOffset;
            CommObjectFactory20 factory = new CommObjectFactory20();
            XGCommLib.DeviceInfo oDevice;

            Lock();
            this.m_CommDriver.RemoveAll();

            for (lCount = 0; lCount < lSizeBit; lCount++)
            {
                oDevice = factory.CreateDevice();

                oDevice.ucDataType = (byte)'X';
                oDevice.ucDeviceType = (byte)szDeviceType;

                lByteOffset = (lOffsetBit + lCount) / 8;	// byte offset
                lBitOffset = (lOffsetBit + lCount) % 8;	// bit offset

                oDevice.lOffset = (Int32)lByteOffset;
                oDevice.lSize = (Int32)lBitOffset;

                this.m_CommDriver.AddDeviceInfo(oDevice);
            }

            byte[] bufRead = new byte[lSizeBit];
            lRetValue = this.m_CommDriver.ReadRandomDevice(bufRead);
            if (0 == lRetValue)
            {
                //++ 재 연결 시도
                dwReteurn = Connect(m_strIP, m_lPortNo);
                if (dwReteurn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    lRetValue = this.m_CommDriver.ReadRandomDevice(bufRead);
                    if (0 == lRetValue)
                    {
                        UnLock();
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_READ;
                    }
                }
                else
                {
                    UnLock();
                    return dwReteurn;
                }
            }
            UnLock();

            if (pbyRead != null)
            {
                bufRead.CopyTo(pbyRead, 0);
            }

            m_nLastCommTime = Environment.TickCount;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        // -> Bit 단위로 지정한 Offset부터 lSizeBit 갯수만큼 쓰기를 수행함
        //    ※ Connect 함수가 최초 호출된 이후에는 연결끊김 발생시 함수 내부에서 1회 자동 연결 시도함(연결안 될 경우 110ms정도 후 리턴됨)    
        //  > szDeviceType    : 디바이스 타입을 지정함, Ex) 'M', 'W', 'R', ...
        //  > lOffsetBit      : 쓰기를 시작할 Bit의 offset을 지정함, Ex) %MX1000 이면 1000
        //  > lSizeBit        : 한번에 쓰기할 Bit 개수를 지정함, 최대 32개, Ex) %MX100, %MX101에 쓰려면, lOffsetBit에 100을 지정하고 lSizeBit는 2를 지정
        //  > *pbyWrite       : 쓰기할 값이 저장된 Bit 배열 포인트를 지정, 반드시 배열의 갯수는 lSizeBit 보다 같거나 커야됨 
        public uint WriteDataBit(char szDeviceType, long lOffsetBit, long lSizeBit, Byte[] pbyWrite)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            if (lSizeBit > (uint)XGCOMM_PRE_DEFINES.MAX_RW_BIT_SIZE)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_BIT_SIZE;
            }

            if (lSizeBit < 1)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_BLOW_MIN_SIZE;
            }

            uint dwReteurn;
            long lRetValue = 0, lCount = 0, lByteOffset, lBitOffset;
            CommObjectFactory20 factory = new CommObjectFactory20();

            Lock();
            this.m_CommDriver.RemoveAll();

            for (lCount = 0; lCount < lSizeBit; lCount++)
            {
                XGCommLib.DeviceInfo oDevice = factory.CreateDevice();

                oDevice.ucDataType = (byte)'X';
                oDevice.ucDeviceType = (byte)szDeviceType;

                lByteOffset = (lOffsetBit + lCount) / 8;	// byte offset
                lBitOffset = (lOffsetBit + lCount) % 8;	    // bit offset

                oDevice.lOffset = (Int32)lByteOffset;
                oDevice.lSize = (Int32)lBitOffset;

                this.m_CommDriver.AddDeviceInfo(oDevice);
            }

            lRetValue = this.m_CommDriver.WriteRandomDevice(pbyWrite);
            if (0 == lRetValue)
            {
                //++ 재 연결 시도
                dwReteurn = Connect(m_strIP, m_lPortNo);
                if (dwReteurn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    lRetValue = this.m_CommDriver.ReadRandomDevice(pbyWrite);
                    if (0 == lRetValue)
                    {
                        UnLock();
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_WRITE;
                    }
                }
                else
                {
                    UnLock();
                    return dwReteurn;
                }
            }
            UnLock();

            m_nLastCommTime = Environment.TickCount;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        // -> Byte 단위로 지정한 Offset부터 lSizeByte 갯수만큼 읽기를 수행함
        //    ※ Connect 함수가 최초 호출된 이후에는 연결끊김 발생시 함수 내부에서 1회 자동 연결 시도함(연결안 될 경우 110ms정도 후 리턴됨)    
        //  - szDeviceType    : 디바이스 타입을 지정함, Ex) 'M', 'W', 'R', ...
        //  - lOffsetByte     : 읽기를 시작할 Byte의 offset을 지정함, Ex) %MB100 이면 100
        //  - lSizeByte       : 한번에 읽기할 Byte 개수를 지정함, Ex) MB100, MB101에 쓰려면, lOffsetByte에 100을 지정하고 lSizeByte는 2를 지정
        //  - *pbyRead        : 읽기할 값이 저장 될 Byte 배열 포인트를 지정, 반드시 배열의 갯수는 lSizeByte 보다 같거나 커야됨 
        public uint ReadDataByte(char szDeviceType, long lOffsetByte, long lSizeByte, Byte[] pbyRead)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            if (lSizeByte > (uint)XGCOMM_PRE_DEFINES.MAX_RW_BYTE_SIZE)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_BYTE_SIZE;
            }

            if (lSizeByte < 1)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_BLOW_MIN_SIZE;
            }

            uint dwReteurn;
            long lRetValue = 0;
            CommObjectFactory20 factory = new CommObjectFactory20();

            Lock();
            this.m_CommDriver.RemoveAll();

            XGCommLib.DeviceInfo oDevice = factory.CreateDevice();

            oDevice.ucDataType = (byte)'B';
            oDevice.ucDeviceType = (byte)szDeviceType;

            oDevice.lOffset = (Int32)lOffsetByte;
            oDevice.lSize = (Int32)lSizeByte;

            this.m_CommDriver.AddDeviceInfo(oDevice);

            byte[] bufRead = new byte[lSizeByte];
            lRetValue = this.m_CommDriver.ReadRandomDevice(bufRead);
            if (0 == lRetValue)
            {
                //++ 재 연결 시도
                dwReteurn = Connect(m_strIP, m_lPortNo);
                if (dwReteurn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    lRetValue = this.m_CommDriver.ReadRandomDevice(bufRead);
                    if (0 == lRetValue)
                    {
                        UnLock();
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_READ;
                    }
                }
                else
                {
                    UnLock();
                    return dwReteurn;
                }
            }
            UnLock();

            if (pbyRead != null)
            {
                bufRead.CopyTo(pbyRead, 0);
            }

            m_nLastCommTime = Environment.TickCount;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        // -> Byte 단위로 지정한 Offset부터 lSizeByte 갯수만큼 쓰기를 수행함
        //    ※ Connect 함수가 최초 호출된 이후에는 연결끊김 발생시 함수 내부에서 1회 자동 연결 시도함(연결안 될 경우 110ms정도 후 리턴됨)   
        //  - szDeviceType    : 디바이스 타입을 지정함, Ex) 'M', 'W', 'R', ...
        //  - lOffsetByte     : 쓰기를 시작할 Byte의 offset을 지정함, Ex) MB100 이면 100
        //  - lSizeByte       : 한번에 쓰기할 Byte 개수를 지정함, Ex) %MB100, %MB101에 쓰려면, lOffsetByte에 100을 지정하고 lSizeByte는 2를 지정
        //  - *pbyWrite       : 쓰기할 값이 저장 된 Byte 배열 포인트를 지정, 반드시 배열의 갯수는 lSizeByte 보다 같거나 커야됨 
        public uint WriteDataByte(char szDeviceType, long lOffsetByte, long lSizeByte, Byte[] pbyWrite)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            if (lSizeByte > (uint)XGCOMM_PRE_DEFINES.MAX_RW_BYTE_SIZE)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_BYTE_SIZE;
            }

            if (lSizeByte < 1)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_BLOW_MIN_SIZE;
            }

            if (pbyWrite == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_POINT;
            }

            uint dwReteurn;
            long lRetValue = 0;
            CommObjectFactory20 factory = new CommObjectFactory20();

            Lock();
            this.m_CommDriver.RemoveAll();

            XGCommLib.DeviceInfo oDevice = factory.CreateDevice();

            oDevice.ucDataType = (byte)'B';
            oDevice.ucDeviceType = (byte)szDeviceType;

            oDevice.lOffset = (Int32)lOffsetByte;
            oDevice.lSize = (Int32)lSizeByte;

            this.m_CommDriver.AddDeviceInfo(oDevice);

            lRetValue = this.m_CommDriver.WriteRandomDevice(pbyWrite);
            if (0 == lRetValue)
            {
                //++ 재 연결 시도
                dwReteurn = Connect(m_strIP, m_lPortNo);
                if (dwReteurn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    lRetValue = this.m_CommDriver.WriteRandomDevice(pbyWrite);
                    if (0 == lRetValue)
                    {
                        UnLock();
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_WRITE;
                    }
                }
                else
                {
                    UnLock();
                    return dwReteurn;
                }
            }
            UnLock();

            m_nLastCommTime = Environment.TickCount;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }

        // -> WORD 단위로 지정한 Offset부터 lSizeWord 갯수만큼 읽기를 수행함
        //    ※ Connect 함수가 최초 호출된 이후에는 연결끊김 발생시 함수 내부에서 1회 자동 연결 시도함(연결안 될 경우 110ms정도 후 리턴됨)    
        //  - szDeviceType    : 디바이스 타입을 지정함, Ex) 'M', 'W', 'R', ...
        //  - lOffsetByte     : 읽기를 시작할 Word의 offset을 지정함, Ex) %MW100 이면 100
        //  - lSizeWord       : 한번에 읽기할 Word 개수를 지정함, Ex) MW100, MW101에 값을 읽으려면, lOffsetWord에 100을 지정하고 lSizeWord는 2를 지정
        //  - bByteSwap       : 읽은 값의 하위/상위 Byte Swap여부를 지정
        //  - *pwRead         : 읽기할 값이 저장 될 Word 배열 포인트를 지정, 반드시 배열의 갯수는 lSizeWord 보다 같거나 커야됨
        public uint ReadDataWord(char szDeviceType, long lOffsetWord, long lSizeWord, bool bByteSwap, UInt16[] pwRead)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            if (lSizeWord > (uint)XGCOMM_PRE_DEFINES.MAX_RW_WORD_SIZE)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_WORD_SIZE;
            }

            if (lSizeWord < 1)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_BLOW_MIN_SIZE;
            }

            uint dwReturn;
            long lCount, lOffsetByte, lSizeByte, lByteOffset;

            lOffsetByte = lOffsetWord * 2;
            lSizeByte = lSizeWord * 2;

            byte[] bufRead = new byte[lSizeByte];

            dwReturn = ReadDataByte(szDeviceType, lOffsetByte, lSizeByte, bufRead);
            if (dwReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
            {
                if (pwRead != null)
                {
                    if (bByteSwap == true)
                    {
                        for (lCount = 0; lCount < lSizeWord; lCount++)
                        {
                            lByteOffset = lCount * 2;
                            pwRead[lCount] = MAKEWORD(bufRead[lByteOffset + 1], bufRead[lByteOffset]);
                        }
                    }
                    else
                    {
                        System.Buffer.BlockCopy(bufRead, 0, pwRead, 0, (Int32)lSizeByte);
                    }
                }
            }

            if (dwReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
            {
                m_nLastCommTime = Environment.TickCount;
            }
            return dwReturn;
        }

        // -> Word 단위로 지정한 Offset부터 lSizeWord 갯수만큼 쓰기를 수행함
        //    ※ Connect 함수가 최초 호출된 이후에는 연결끊김 발생시 함수 내부에서 1회 자동 연결 시도함(연결안 될 경우 110ms정도 후 리턴됨)   
        //  - szDeviceType    : 디바이스 타입을 지정함, Ex) 'M', 'W', 'R', ...
        //  - lOffsetByte     : 쓰기를 시작할 Word의 offset을 지정함, Ex) MW100 이면 100
        //  - lSizeByte       : 한번에 쓰기할 Word 개수를 지정함, Ex) %MW100, %MW101에 쓰려면, lOffsetWord에 100을 지정하고 lSizeWord는 2를 지정
        //  - *pbyWrite       : 쓰기할 값이 저장 된 Word 배열 포인트를 지정, 반드시 배열의 갯수는 lSizeWord 보다 같거나 커야됨 
        public uint WriteDataWord(char szDeviceType, long lOffsetWord, long lSizeWord, bool bByteSwap, UInt16[] pwWrite)
        {
            if (this.m_CommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            if (lSizeWord > (uint)XGCOMM_PRE_DEFINES.MAX_RW_WORD_SIZE)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_WORD_SIZE;
            }

            if (lSizeWord < 1)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_BLOW_MIN_SIZE;
            }

            if (pwWrite == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_POINT;
            }

            uint dwReturn;
            long lCount, lOffsetByte, lSizeByte, lByteOffset;

            lOffsetByte = lOffsetWord * 2;
            lSizeByte = lSizeWord * 2;

            byte[] bufWrite = new byte[lSizeByte];

            if (bByteSwap == true)
            {
                for (lCount = 0; lCount < lSizeWord; lCount++)
                {
                    lByteOffset = lCount * 2;

                    bufWrite[lByteOffset] = HIBYTE(pwWrite[lCount]);
                    bufWrite[lByteOffset + 1] = LOBYTE(pwWrite[lCount]);
                }
            }
            else
            {
                System.Buffer.BlockCopy(pwWrite, 0, bufWrite, 0, (Int32)lSizeByte);
            }

            dwReturn = WriteDataByte(szDeviceType, lOffsetByte, lSizeByte, bufWrite);
            if (dwReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
            {
                m_nLastCommTime = Environment.TickCount;
            }

            return dwReturn;
        }

        // -> 각 함수 실행후 반환되는 리턴값에 대한 문자열 반환
        //  - dwReturnCode  : 함수의 반환 코드를 지정, Ex) dwReturnCode = WriteDataWord('M', 100, 2, FALSE, pwWrite);
        public string GetReturnCodeString(uint uReturnCode)
        {
            string strMsg = "";

            switch (uReturnCode)
            {
                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS:
                    strMsg = "Success";
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_CAN_NOT_FIND_DLL:
                    strMsg = ("Can not find \"XGCommLib.dll\"");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_CONNECT:
                    strMsg = ("Connect failed");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_KEEPALIVE:
                    strMsg = ("Keep alive failed");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER:
                    strMsg = ("Invalid comm driver");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_POINT:
                    strMsg = ("Invalid data point");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_RESULT:
                    strMsg = ("Failed comm driver");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_READ:
                    strMsg = ("Read data failed");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_WRITE:
                    strMsg = ("Write data failed");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_BIT_SIZE:
                    strMsg = ("Above max bit size[Max bit: 32]");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_BYTE_SIZE:
                    strMsg = ("Above max byte size[Max byte: 260]");
                    break;

                case (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_WORD_SIZE:
                    strMsg = ("Above max word size[Max word: 130]");
                    break;

                default:
                    strMsg = ("Unknown return code");
                    break;
            }

            return strMsg;
        }

        // -> TCP통신 접속 대기 시간 반환함
        //  - *pdwTimeout     : 설정된 접속 대기 시간을 반환 
        public uint GetCommTimeout(ref uint? pdwTimeout)
        {
            var rkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\XGLibConfig");

            if (rkey != null)
            {
                var strValue = rkey.GetValue("Timeout")!.ToString();

                if (pdwTimeout != null)
                    pdwTimeout = Convert.ToUInt16(strValue);

                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
            }

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_GET_TIMEOUT;
        }

        // -> TCP통신 접속 대기 시간 설정함 (ms단위)
        //  - dwTimeout       : 설정할 접속 대기 시간을 지정
        public uint SetCommTimeout(uint dwTimeout)
        {
            var rkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\XGLibConfig", true);

            if (rkey != null)
            {
                rkey.SetValue("Timeout", dwTimeout);
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
            }

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_SET_TIMEOUT;
        }

        //=============================================================================
        private static byte LOBYTE(UInt16 a)
        {
            return ((byte)(a & 0xff));
        }

        private static byte HIBYTE(UInt16 a)
        {
            return ((byte)(a >> 8));
        }

        private static UInt16 MAKEWORD(byte low, byte high)
        {
            return (UInt16)((high << 8) | low);
        }

        private static Int32 TICKS_DIFF(int prev, int cur)
        {
            Int32 nReturn;
            if (cur >= prev)
            {
                nReturn = cur - prev;
            }
            else
            {
                unchecked
                {
                    nReturn = ((int)0xFFFFFFFF - prev) + 1 + cur;
                }
            }
            return nReturn;
        }

        private void Lock()
        {
            Monitor.Enter(m_MonitorLock);
        }

        private void UnLock()
        {
            Monitor.Exit(m_MonitorLock);
        }
    }
}
