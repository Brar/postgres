using System;

namespace PlClr
{
    public static class ServerMemory
    {
        private static bool _isInitialized;
        private static readonly object LockObject = new object();
        private static PAllocDelegate? _pallocDelegate;
        private static PAllocDelegate? _palloc0Delegate;
        private static RePAllocDelegate? _repallocDelegate;
        private static PFreeDelegate? _pfreeDelegate;

        internal static void Initialize(
            PAllocDelegate pallocDelegate,
            PAllocDelegate palloc0Delegate,
            RePAllocDelegate repallocDelegate,
            PFreeDelegate pfreeDelegate
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
