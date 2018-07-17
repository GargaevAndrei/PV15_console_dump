namespace ControlPoint
{
    /// <summary>
    /// Число информационных бит в байте.
    /// </summary>
    public enum ByteSize : byte
    {
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    };

    /// <summary>
    /// Скорости передачи данных.
    /// </summary>
    public enum BaudRate : int
    {
        Baud_110 = 110,
        Baud_300 = 300,
        Baud_600 = 600,
        Baud_1200 = 1200,
        Baud_2400 = 2400,
        Baud_4800 = 4800,
        Baud_9600 = 9600,
        Baud_14400 = 14400,
        Baud_19200 = 19200,
        Baud_28800 = 28800,
        Baud_38400 = 38400,
        Baud_56000 = 56000,
        Baud_57600 = 57600,
        Baud_115200 = 115200,
        Baud_128000 = 128000,
        Baud_256000 = 256000,
    };

    /// <summary>
    /// Установки четности.
    /// </summary>
    public enum Parity : byte
    {
        /// <summary>
        /// Без бита четности.
        /// </summary>
        None = 0,

        /// <summary>
        /// Дополнение до нечетности.
        /// </summary>
        Odd = 1,

        /// <summary>
        /// Дополнение до четности.
        /// </summary>
        Even = 2,

        /// <summary>
        /// Бит четности всегда 1.
        /// </summary>
        Mark = 3,

        /// <summary>
        /// Бит четности всегда 0.
        /// </summary>
        Space = 4
    };

    /// <summary>
    /// Количество стоповых бит
    /// </summary>
    public enum StopBits
    {
        /// <summary>
        /// Один стоповый бит
        /// </summary>
        One = 0,

        /// <summary>
        /// Полтора стоповых бита
        /// </summary>
        OnePointFive = 1,

        /// <summary>
        /// Два стоповых бита
        /// </summary>
        Two = 2
    };
}
