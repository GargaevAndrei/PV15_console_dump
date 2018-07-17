using System;

namespace ControlPoint
{
    public sealed class SerialPort : IDisposable
    {
        #region FIELDS
        private readonly byte[] bytes = new byte[64000];
        //признак освобождения объекта
        private bool disposed = false;
        //хэндл порта
        private IntPtr handle = (IntPtr)SerialPortWinApi.INVALID_HANDLE_VALUE;
        //DCB
        private SerialPortWinApi.DCB dcb = new SerialPortWinApi.DCB();
        //таймауты порта
        private SerialPortWinApi.COMMTIMEOUTS commtimeouts = new SerialPortWinApi.COMMTIMEOUTS();
        //признак активности порта
        private volatile bool active = false;
        //имя порта
        private string portname = String.Empty;
        //скорость
        private BaudRate baudrate = BaudRate.Baud_110;
        //паритет
        private Parity parity = Parity.None;
        //стоповые биты
        private StopBits stopbits = StopBits.One;
        //размер байта
        private ByteSize bytesize = ByteSize.Eight;
        //размер входного буфера
        private uint sizeinbuf = 0x1000;
        //размер выходного буфера
        private uint sizeoutbuf = 0x1000;

        #endregion

        #region PROPERTIES

        //флаг состояния
        public bool Active
        {
            get
            {

                return this.active;
            }
        }
        //хэндл порта
        public IntPtr Handle
        {
            get
            {

                return this.handle;
            }
        }
        //имя порта
        public string PortName
        {
            get
            {

                return this.portname;
            }
        }
        //скорость
        public BaudRate BaudRate
        {
            get
            {

                return this.baudrate;
            }
        }
        //паритет
        public Parity Parity
        {
            get
            {

                return this.parity;
            }
        }
        //размер байта
        public ByteSize ByteSize
        {
            get
            {

                return this.bytesize;
            }
        }
        //кол-во стоп бит
        public StopBits StopBits
        {
            get
            {

                return this.stopbits;
            }
        }
        //размер буфера приема
        public uint SizeInBuf
        {
            get
            {

                return this.sizeinbuf;
            }

            set
            {

                {
                    if (this.sizeinbuf == value || this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return;

                    this.sizeinbuf = value;

                    if (!SerialPortWinApi.SetupComm(this.handle, this.sizeinbuf, this.sizeoutbuf))
                        throw new Exception();//!!!
                }
            }
        }
        //размер буфера передачи
        public uint SizeOutBuf
        {
            get
            {

                return this.sizeoutbuf;
            }

            set
            {

                {
                    if (this.sizeoutbuf == value || this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return;

                    this.sizeoutbuf = value;

                    if (!SerialPortWinApi.SetupComm(this.handle, this.sizeinbuf, this.sizeoutbuf))
                        throw new Exception();//!!!
                }
            }
        }
        //байт в буфере приема
        public int ByteInBuffer
        {
            get
            {

                {
                    uint errors = 0;
                    SerialPortWinApi.COMSTAT state = new SerialPortWinApi.COMSTAT();

                    if (SerialPortWinApi.ClearCommError(this.handle, out errors, out state))
                        return (int)state.cbInQue;
                    else
                        return -1;
                }
            }
        }
        //таймауты порта
        public uint ReadIntervalTimeout
        {
            get
            {

                return this.commtimeouts.ReadIntervalTimeout;
            }

            set
            {

                {
                    if (this.commtimeouts.ReadIntervalTimeout == value || this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return;

                    this.commtimeouts.ReadIntervalTimeout = value;

                    if (!SerialPortWinApi.SetCommTimeouts(this.handle, ref this.commtimeouts))
                        throw new Exception();//!!!
                }
            }
        }
        public uint ReadTotalTimeoutMultiplier
        {
            get
            {

                return this.commtimeouts.ReadTotalTimeoutMultiplier;
            }

            set
            {

                {
                    if (this.commtimeouts.ReadTotalTimeoutMultiplier == value || this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return;

                    this.commtimeouts.ReadTotalTimeoutMultiplier = value;

                    if (!SerialPortWinApi.SetCommTimeouts(this.handle, ref this.commtimeouts))
                        throw new Exception();//!!!
                }
            }
        }
        public uint WriteTotalTimeoutConstant
        {
            get
            {

                return this.commtimeouts.WriteTotalTimeoutConstant;
            }

            set
            {

                {
                    if (this.commtimeouts.WriteTotalTimeoutConstant == value || this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return;

                    this.commtimeouts.WriteTotalTimeoutConstant = value;

                    if (!SerialPortWinApi.SetCommTimeouts(this.handle, ref this.commtimeouts))
                        throw new Exception();//!!!
                }
            }
        }
        public uint WriteTotalTimeoutMultiplier
        {
            get
            {

                return this.commtimeouts.WriteTotalTimeoutMultiplier;
            }

            set
            {

                {
                    if (this.commtimeouts.WriteTotalTimeoutMultiplier == value || this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return;

                    this.commtimeouts.WriteTotalTimeoutMultiplier = value;

                    if (!SerialPortWinApi.SetCommTimeouts(this.handle, ref this.commtimeouts))
                        throw new Exception();//!!!
                }
            }
        }
        public uint ReadTotalTimeoutConstant
        {
            get
            {

                return this.commtimeouts.ReadTotalTimeoutConstant;
            }

            set
            {

                {
                    if (this.commtimeouts.ReadTotalTimeoutConstant == value || this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return;

                    this.commtimeouts.ReadTotalTimeoutConstant = value;

                    if (!SerialPortWinApi.SetCommTimeouts(this.handle, ref this.commtimeouts))
                        throw new Exception();//!!!
                }
            }
        }

        #endregion

        #region METHODS
        //очистить буферы порта
        public void Clear()
        {
            if (this.handle.ToInt32() != SerialPortWinApi.INVALID_HANDLE_VALUE)
                SerialPortWinApi.PurgeComm(this.handle, (SerialPortWinApi.PURGE_RXCLEAR | SerialPortWinApi.PURGE_TXCLEAR));
        }
        //закрыть порт
        public void Close()
        {
            if (this.handle.ToInt32() != SerialPortWinApi.INVALID_HANDLE_VALUE && this.active)
            {
                //SerialPortWinApi.CancelIo(this.handle);
                SerialPortWinApi.PurgeComm(this.handle, (SerialPortWinApi.PURGE_RXCLEAR | SerialPortWinApi.PURGE_TXCLEAR));
                SerialPortWinApi.CloseHandle(this.handle);

                this.handle = (IntPtr)SerialPortWinApi.INVALID_HANDLE_VALUE;
                this.baudrate = BaudRate.Baud_110;
                this.portname = String.Empty;
                this.stopbits = StopBits.One;
                this.parity = Parity.None;
                this.bytesize = ByteSize.Eight;
                this.sizeinbuf = 0x1000;
                this.sizeoutbuf = 0x1000;
                this.commtimeouts = new SerialPortWinApi.COMMTIMEOUTS();
                this.dcb = new SerialPortWinApi.DCB();

            }
            this.active = false;
        }
        //открыть порт
        public bool Open(string portname, BaudRate baudrate, Parity parity, StopBits stopbits, ByteSize bytesize)
        {
            try
            {
                if (this.active) return false;

                this.handle = SerialPortWinApi.CreateFile(String.Format("{0}", portname), SerialPortWinApi.GENERIC_READ | SerialPortWinApi.GENERIC_WRITE, 0, IntPtr.Zero, SerialPortWinApi.OPEN_EXISTING, SerialPortWinApi.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

                if (this.handle.ToInt32() == SerialPortWinApi.INVALID_HANDLE_VALUE) return false;

                if (!SerialPortWinApi.SetupComm(this.handle, this.sizeinbuf, this.sizeoutbuf))
                    throw new Exception();//!!!

                if (!SerialPortWinApi.GetCommState(this.handle, ref this.dcb))
                    throw new Exception();//!!!

                this.dcb.BaudRate = (int)baudrate;
                this.dcb.Parity = (byte)parity;
                this.dcb.ByteSize = (byte)bytesize;
                this.dcb.StopBits = (byte)stopbits;

                if (!SerialPortWinApi.SetCommState(this.handle, ref this.dcb))
                    throw new Exception();//!!

                this.commtimeouts.ReadIntervalTimeout = 0;
                this.commtimeouts.ReadTotalTimeoutMultiplier = 0;
                this.commtimeouts.ReadTotalTimeoutConstant = 1000;
                this.commtimeouts.WriteTotalTimeoutConstant = 1000;
                this.commtimeouts.WriteTotalTimeoutMultiplier = 0;

                if (!SerialPortWinApi.SetCommTimeouts(this.handle, ref this.commtimeouts))
                    throw new Exception();//!!

                this.active = true;

                return true;
            }
            catch
            {
                this.handle = (IntPtr)SerialPortWinApi.INVALID_HANDLE_VALUE;
                this.baudrate = BaudRate.Baud_110;
                this.portname = String.Empty;
                this.stopbits = StopBits.One;
                this.parity = Parity.None;
                this.bytesize = ByteSize.Eight;
                this.sizeinbuf = 0x1000;
                this.sizeoutbuf = 0x1000;
                this.commtimeouts = new SerialPortWinApi.COMMTIMEOUTS();
                this.dcb = new SerialPortWinApi.DCB();

                return false;
            }
        }
        //записать данные в порт
        public bool Write(byte[] data, uint NumberOfBytesToWrite, ref uint NumberOfBytesWritten)
        {
            if (this.handle.ToInt32() != SerialPortWinApi.INVALID_HANDLE_VALUE)
            {
                if (!SerialPortWinApi.WriteFile(this.handle, data, NumberOfBytesToWrite, out NumberOfBytesWritten, IntPtr.Zero))
                    return false;

                return (NumberOfBytesToWrite == NumberOfBytesWritten);
            }
            else return false;
        }
        //считать данные с порта
        public bool Read(byte[] buffer, uint NumberOfBytesToRead, ref uint NumberOfBytesRead)
        {
            if (this.handle.ToInt32() != SerialPortWinApi.INVALID_HANDLE_VALUE)
            {
                if (!SerialPortWinApi.ReadFile(this.handle, buffer, NumberOfBytesToRead, out NumberOfBytesRead, IntPtr.Zero))
                    return false;

                return (NumberOfBytesToRead == NumberOfBytesRead);
            }
            else return false;
        }
        //считать данные с порта, NumberOfBytesToRead<=64000
        public int Read(byte[] buffer, uint offset, uint NumberOfBytesToRead)
        {
            if (this.handle.ToInt32() != SerialPortWinApi.INVALID_HANDLE_VALUE)
            {
                uint NumberOfBytesRead = 0;
                //byte[] tmp = new byte[NumberOfBytesToRead];

                if (!SerialPortWinApi.ReadFile(this.handle, bytes, NumberOfBytesToRead, out NumberOfBytesRead, IntPtr.Zero))
                    return -1;

                Array.Copy(bytes, 0, buffer, (int)offset, (int)NumberOfBytesRead);
                //tmp = null;

                return (int)NumberOfBytesRead;
            }
            else return -1;
        }

        //освобождение объекта
        ~SerialPort()
        {
            Dispose(false);
        }
        //освобождение объекта
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //освобождение объекта
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (active)
                    Close();
            }
            disposed = true;
        }

        #endregion
    }
}
