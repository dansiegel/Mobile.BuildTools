using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mobile.BuildTools
{
    public class AnsiTerminal : IDisposable
    {

        // Great resource about ANSI codes and how to use them:
        // http://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html

        //--- Constants ---
        public const string Black = "\u001b[30m";
        public const string Red = "\u001b[31m";
        public const string Green = "\u001b[32m";
        public const string Yellow = "\u001b[33m";
        public const string Blue = "\u001b[34m";
        public const string Magenta = "\u001b[35m";
        public const string Cyan = "\u001b[36m";
        public const string White = "\u001b[37m";
        public const string BrightBlack = "\u001b[30;1m";
        public const string BrightRed = "\u001b[31;1m";
        public const string BrightGreen = "\u001b[32;1m";
        public const string BrightYellow = "\u001b[33;1m";
        public const string BrightBlue = "\u001b[34;1m";
        public const string BrightMagenta = "\u001b[35;1m";
        public const string BrightCyan = "\u001b[36;1m";
        public const string BrightWhite = "\u001b[37;1m";
        public const string BackgroundBlack = "\u001b[40m";
        public const string BackgroundRed = "\u001b[41m";
        public const string BackgroundGreen = "\u001b[42m";
        public const string BackgroundYellow = "\u001b[43m";
        public const string BackgroundBlue = "\u001b[44m";
        public const string BackgroundMagenta = "\u001b[45m";
        public const string BackgroundCyan = "\u001b[46m";
        public const string BackgroundWhite = "\u001b[47m";
        public const string BackgroundBrightBlack = "\u001b[40;1m";
        public const string BackgroundBrightRed = "\u001b[41;1m";
        public const string BackgroundBrightGreen = "\u001b[42;1m";
        public const string BackgroundBrightYellow = "\u001b[43;1m";
        public const string BackgroundBrightBlue = "\u001b[44;1m";
        public const string BackgroundBrightMagenta = "\u001b[45;1m";
        public const string BackgroundBrightCyan = "\u001b[46;1m";
        public const string BackgroundBrightWhite = "\u001b[47;1m";
        public const string Reset = "\u001b[0m";
        public const string ClearEndOfLine = "\u001b[0K";
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        //--- Class Methods ---
        public static string MoveUp(int count) => $"\u001b[{count}A";
        public static string MoveDown(int count) => $"\u001b[{count}B";
        public static string MoveRight(int count) => $"\u001b[{count}C";
        public static string MoveLeft(int count) => $"\u001b[{count}D";

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        //--- Fields ---
        private bool _enableAnsiOutput;
        private bool _switchedToAnsi;
        private IntPtr _consoleStandardOut;
        private uint _originaConsoleMode;

        //--- Constructors ---
        public AnsiTerminal(bool enableAnsiOutput)
        {
            _enableAnsiOutput = enableAnsiOutput;
            if (_enableAnsiOutput && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SwitchWindowsConsoleToAnsi();
            }
        }

        //--- Methods ---
        public void Dispose()
        {
            RestoreWindowsConsoleSettings();
        }

        private void SwitchWindowsConsoleToAnsi()
        {
            _consoleStandardOut = GetStdHandle(STD_OUTPUT_HANDLE);
            _switchedToAnsi = GetConsoleMode(_consoleStandardOut, out _originaConsoleMode)
                && SetConsoleMode(_consoleStandardOut, _originaConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);
        }

        private void RestoreWindowsConsoleSettings()
        {
            if (_switchedToAnsi)
            {
                SetConsoleMode(_consoleStandardOut, _originaConsoleMode);
            }
        }
    }
}
