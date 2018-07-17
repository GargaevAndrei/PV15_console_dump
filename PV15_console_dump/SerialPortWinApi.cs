using System;
using System.Runtime.InteropServices;

namespace ControlPoint
{
    internal class SerialPortWinApi
    {
        const string dll_name = "kernel32.dll";

        #region Функции для открытия, тестирования и закрытия порта.
        [DllImport(dll_name, SetLastError = true)]
        internal static extern IntPtr CreateFile(string lpFileName,
            uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        //Константы ошибок:
        internal const uint ERROR_FILE_NOT_FOUND = 2;
        internal const uint ERROR_INVALID_NAME = 123;
        internal const uint ERROR_ACCESS_DENIED = 5;
        internal const uint ERROR_IO_PENDING = 997;

        //Код возврата:
        internal const int INVALID_HANDLE_VALUE = -1;

        //Константы для dwFlagsAndAttributes:
        internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        //Константы для dwCreationDisposition:
        internal const uint OPEN_EXISTING = 3;

        //Константы для dwDesiredAccess:
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;

        [DllImport(dll_name)]
        internal static extern bool CloseHandle(IntPtr hObject);
        #endregion Функции для открытия, тестирования и закрытия порта.

        #region Функции установки параметров связи.
        [DllImport(dll_name)]
        internal static extern bool GetCommState(IntPtr hFile,
            ref DCB lpDCB);

        [DllImport(dll_name)]
        internal static extern bool GetCommTimeouts(IntPtr hFile,
            out COMMTIMEOUTS lpComTimeouts);

        [DllImport(dll_name)]
        internal static extern bool BuildCommDCBAndTimeouts(string lpDef,
            ref DCB lpDCB, ref COMMTIMEOUTS lpComTimeouts);

        [DllImport(dll_name)]
        internal static extern bool SetCommState(IntPtr hFile,
            [In] ref DCB lpDCB);

        [DllImport(dll_name)]
        internal static extern bool SetCommTimeouts(IntPtr hFile,
            [In] ref COMMTIMEOUTS lpComTimeouts);

        [DllImport(dll_name)]
        internal static extern bool SetupComm(IntPtr hFile, uint dwInQueue,
            uint dwOutQueue);

        [StructLayout(LayoutKind.Sequential)]
        internal struct COMMTIMEOUTS
        {
            internal uint ReadIntervalTimeout;
            internal uint ReadTotalTimeoutMultiplier;
            internal uint ReadTotalTimeoutConstant;
            internal uint WriteTotalTimeoutMultiplier;
            internal uint WriteTotalTimeoutConstant;
        }
        //Для таймаута немедленного возврата.
        internal const uint MAXDWORD = 0xffffffff;

        [StructLayout(LayoutKind.Sequential)]
        internal struct DCB
        {
            internal int DCBlength;
            internal int BaudRate;
            internal int PackedValues;
            internal short wReserved;
            internal short XonLim;
            internal short XoffLim;
            internal byte ByteSize;
            internal byte Parity;
            internal byte StopBits;
            internal byte XonChar;
            internal byte XoffChar;
            internal byte ErrorChar;
            internal byte EofChar;
            internal byte EvtChar;
            internal short wReserved1;

            internal void init(bool parity, bool outCTS, bool outDSR,
                int dtr, bool inDSR, bool txc, bool xOut,
                bool xIn, int rts)
            {
                DCBlength = 28;
                PackedValues = 0x8001;
                if (parity)
                {
                    PackedValues |= 0x0002;
                }
                if (outCTS)
                {
                    PackedValues |= 0x0004;
                }
                if (outDSR)
                {
                    PackedValues |= 0x0008;
                }
                PackedValues |= ((dtr & 0x0003) << 4);
                if (inDSR)
                {
                    PackedValues |= 0x0040;
                }
                if (txc)
                {
                    PackedValues |= 0x0080;
                }
                if (xOut)
                {
                    PackedValues |= 0x0100;
                }
                if (xIn)
                {
                    PackedValues |= 0x0200;
                }
                PackedValues |= ((rts & 0x0003) << 12);
            }
        }
        #endregion Функции установки параметров связи.

        #region Функции чтения и записи.
        [DllImport(dll_name, SetLastError = true)]
        internal static extern bool WriteFile(IntPtr fFile, byte[] lpBuffer,
            uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [StructLayout(LayoutKind.Sequential)]
        internal struct OVERLAPPED
        {
            internal UIntPtr Internal;
            internal UIntPtr InternalHigh;
            internal uint Offset;
            internal uint OffsetHigh;
            internal IntPtr hEvent;
        }

        [DllImport(dll_name)]
        internal static extern bool SetCommMask(IntPtr hFile,
            uint dwEvtMask);

        //Константы для dwEvtMask:
        internal const uint EV_RXCHAR = 0x0001;
        internal const uint EV_RXFLAG = 0x0002;
        internal const uint EV_TXEMPTY = 0x0004;
        internal const uint EV_CTS = 0x0008;
        internal const uint EV_DSR = 0x0010;
        internal const uint EV_RLSD = 0x0020;
        internal const uint EV_BREAK = 0x0040;
        internal const uint EV_ERR = 0x0080;
        internal const uint EV_RING = 0x0100;
        internal const uint EV_PERR = 0x0200;
        internal const uint EV_RX80FULL = 0x0400;
        internal const uint EV_EVENT1 = 0x0800;
        internal const uint EV_EVENT2 = 0x1000;

        [DllImport(dll_name, SetLastError = true)]
        internal static extern bool WaitCommEvent(IntPtr hFile,
            IntPtr lpEvtMask, IntPtr lpOverlapped);

        [DllImport(dll_name)]
        internal static extern bool CancelIo(IntPtr hFile);

        [DllImport(dll_name, SetLastError = true)]
        internal static extern bool ReadFile(IntPtr hFile,
            [Out] byte[] lpBuffer, uint nNumberOfBytesToRead,
            out uint nNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport(dll_name)]
        internal static extern bool TransmitCommChar(IntPtr hFile,
            byte cChar);
        #endregion Функции чтения и записи.

        #region Функции контроля порта.
        [DllImport(dll_name)]
        internal static extern bool EscapeCommFunction(IntPtr hFile,
            uint dwFunc);

        //Константы для dwFunc:
        internal const uint SETXOFF = 1;
        internal const uint SETXON = 2;
        internal const uint SETRTS = 3;
        internal const uint CLRRTS = 4;
        internal const uint SETDTR = 5;
        internal const uint CLRDTR = 6;
        internal const uint RESETDEV = 7;
        internal const uint SETBREAK = 8;
        internal const uint CLRBREAK = 9;

        [DllImport(dll_name)]
        internal static extern bool GetCommModemStatus(IntPtr hFile,
            out uint lpModemStat);

        //Константы для lpModemStat:
        internal const uint MS_CTS_ON = 0x0010;
        internal const uint MS_DSR_ON = 0x0020;
        internal const uint MS_RING_ON = 0x0040;
        internal const uint MS_RLSD_ON = 0x0080;
        #endregion Функции контроля порта.

        #region Функции статуса.
        [DllImport(dll_name, SetLastError = true)]
        internal static extern bool GetOverlappedResult(IntPtr hFile,
            IntPtr lpOverlapped, out uint nNumberOfBytesTransferred,
            bool bWait);

        [DllImport(dll_name)]
        internal static extern bool ClearCommError(IntPtr hFile,
            out uint lpErrors, IntPtr lpStat);
        [DllImport(dll_name)]
        internal static extern bool ClearCommError(IntPtr hFile,
            out uint lpErrors, out COMSTAT cs);

        //Константы для lpErrors:
        internal const uint CE_RXOVER = 0x0001;
        internal const uint CE_OVERRUN = 0x0002;
        internal const uint CE_RXPARITY = 0x0004;
        internal const uint CE_FRAME = 0x0008;
        internal const uint CE_BREAK = 0x0010;
        internal const uint CE_TXFULL = 0x0100;
        internal const uint CE_PTO = 0x0200;
        internal const uint CE_IOE = 0x0400;
        internal const uint CE_DNS = 0x0800;
        internal const uint CE_OOP = 0x1000;
        internal const uint CE_MODE = 0x8000;

        [StructLayout(LayoutKind.Sequential)]
        internal struct COMSTAT
        {
            internal const uint fCtsHold = 0x1;
            internal const uint fDsrHold = 0x2;
            internal const uint fRlsdHold = 0x4;
            internal const uint fXoffHold = 0x8;
            internal const uint fXoffSent = 0x10;
            internal const uint fEof = 0x20;
            internal const uint fTxim = 0x40;
            internal uint Flags;
            internal uint cbInQue;
            internal uint cbOutQue;
        }
        [DllImport(dll_name)]
        internal static extern bool GetCommProperties(IntPtr hFile,
            out COMMPROP cp);

        [StructLayout(LayoutKind.Sequential)]
        internal struct COMMPROP
        {
            internal ushort wPacketLength;
            internal ushort wPacketVersion;
            internal uint dwServiceMask;
            internal uint dwReserved1;
            internal uint dwMaxTxQueue;
            internal uint dwMaxRxQueue;
            internal uint dwMaxBaud;
            internal uint dwProvSubType;
            internal uint dwProvCapabilities;
            internal uint dwSettableParams;
            internal uint dwSettableBaud;
            internal ushort wSettableData;
            internal ushort wSettableStopParity;
            internal uint dwCurrentTxQueue;
            internal uint dwCurrentRxQueue;
            internal uint dwProvSpec1;
            internal uint dwProvSpec2;
            internal byte wcProvChar;
        }
        #endregion Функции статуса.

        [DllImport(dll_name)]
        internal static extern bool PurgeComm(IntPtr hFile, uint dwFlags);

        internal const uint PURGE_TXABORT = 0x0001;
        internal const uint PURGE_RXABORT = 0x0002;
        internal const uint PURGE_TXCLEAR = 0x0004;
        internal const uint PURGE_RXCLEAR = 0x0008;

    }
}
