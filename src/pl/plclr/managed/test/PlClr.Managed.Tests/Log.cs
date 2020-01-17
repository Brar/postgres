using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class Log : IDisposable
    {
        private bool _disposed;
        private readonly TextWriter _outBackup;
        private readonly TextWriter _errorBackup;
        private readonly List<(SeverityLevel, string?)> _elogMessages;
        private readonly TextWriter _consoleOut;
        private readonly TextWriter _consoleError;
        private readonly PFreeDelegate _pFree;

        public Log(PFreeDelegate pFreeDelegate)
        {
            _pFree = pFreeDelegate;
            _outBackup = Console.Out;
            _errorBackup = Console.Error;
            _elogMessages = new List<(SeverityLevel, string?)>();
            _consoleOut = new StringWriter();
            _consoleError = new StringWriter();
            Console.SetOut(_consoleOut);
            Console.SetError(_consoleError);
        }

        public void Elog(int level, IntPtr message)
        {
            var msg = System.Runtime.InteropServices.Marshal.PtrToStringUTF8(message);
            _elogMessages.Add(((SeverityLevel) level, msg));

            // We have to PFree the string here since we should have allocated it via PAlloc
            // Otherwise our statistics would get messed up.
            _pFree(message);
        }

        public string ConsoleOut
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(Log));

                return _consoleOut.ToString()!;
            }
        }

        public string ConsoleError
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(Log));

                return _consoleError.ToString()!;
            }
        }

        public IEnumerable<(SeverityLevel, string?)> ELogMessages
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(Log));

                return _elogMessages;
            }
        }

        public IntPtr GetELogFunctionPointer()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Log));

            return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ELogDelegate>(Elog);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Console.SetOut(_outBackup);
            Console.SetError(_errorBackup);
            _consoleOut.Dispose();
            _consoleError.Dispose();
            _disposed = true;
        }
    }
}
