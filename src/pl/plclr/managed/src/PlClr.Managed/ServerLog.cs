using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public static class ServerLog
    {
        private static bool _isInitialized;
        private static readonly object LockObject = new object();
        private static Action<int, IntPtr>? _elogDelegate;

        internal static void Initialize(Action<int, IntPtr> elogDelegate)
        {
            if (_isInitialized)
                return;
            lock (LockObject)
            {
                if (_isInitialized)
                    return;

                _elogDelegate = elogDelegate;
                _isInitialized = true;
            }
        }

        public static void ELog(SeverityLevel level, string message)
        {
            _elogDelegate!((int)level, Marshal.StringToPtrPalloc(message));
        }
    }
}
