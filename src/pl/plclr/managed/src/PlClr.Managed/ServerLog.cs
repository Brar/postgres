namespace PlClr
{
    public static class ServerLog
    {
        private static ELogDelegate? _elogDelegate;

        internal static void Initialize(ELogDelegate elogDelegate)
        {
            _elogDelegate = elogDelegate;
        }

        public static void ELog(SeverityLevel level, string message)
        {
            _elogDelegate!((int)level, Marshal.StringToPtrPalloc(message));
        }
    }
}
