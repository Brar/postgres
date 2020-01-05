using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public static class ServerMemory
    {
        private static bool _isInitialized;
        private static readonly object LockObject = new object();
        private static Func<ulong, IntPtr>? _pallocDelegate;
        private static Func<ulong, IntPtr>? _palloc0Delegate;
        private static Func<IntPtr, ulong, IntPtr>? _repallocDelegate;
        private static Action<IntPtr>? _pfreeDelegate;

        internal static void Initialize(
            Func<ulong, IntPtr> pallocDelegate,
            Func<ulong, IntPtr> palloc0Delegate,
            Func<IntPtr, ulong, IntPtr> repallocDelegate,
            Action<IntPtr> pfreeDelegate
        )
        {
            if (_isInitialized)
                return;
            lock (LockObject)
            {
                if (_isInitialized)
                    return;

                _pallocDelegate = pallocDelegate;
                _palloc0Delegate = palloc0Delegate;
                _repallocDelegate = repallocDelegate;
                _pfreeDelegate = pfreeDelegate;
                _isInitialized = true;
            }
        }

        public static IntPtr Palloc(ulong size)
        {
            var ptr = _pallocDelegate!(size);

            if (ptr == IntPtr.Zero)
                throw new InsufficientMemoryException();

            return ptr;
        }

        public static IntPtr Palloc0(ulong size)
        {
            var ptr = _palloc0Delegate!(size);

            if (ptr == IntPtr.Zero)
                throw new InsufficientMemoryException();

            return ptr;
        }

        public static IntPtr Repalloc(IntPtr ptr, ulong size)
        {
            var p = _repallocDelegate!(ptr, size);

            if (p == IntPtr.Zero)
                throw new InsufficientMemoryException();

            return p;
        }

        public static void PFree(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return;

            _pfreeDelegate!(ptr);
        }
    }
}
