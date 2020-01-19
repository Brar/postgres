using System;

namespace PlClr
{
    public static class ServerLog
    {
        private static ELogDelegate? _elogDelegate;
        private static EReportDelegate? _ereportDelegate;

        internal static void Initialize(ELogDelegate elogDelegate, EReportDelegate ereportDelegate)
        {
            _elogDelegate = elogDelegate;
            _ereportDelegate = ereportDelegate;
        }

        public static PlClrUnreachableException? ELog(SeverityLevel level, string message)
        {
            _elogDelegate!((int)level, Marshal.ToPtrPalloc(message));

            return level >= SeverityLevel.Error ? new PlClrUnreachableException() : null;
        }

        /// <summary>
        /// Generates error, warning, and log messages
        /// </summary>
        /// <param name="level">The <see cref="SeverityLevel"/> (ranging from <see cref="SeverityLevel.Debug5"/> to <see cref="SeverityLevel.Panic"/>).</param>
        /// <param name="errorMessageInternal">Specifies the primary error message text,
        /// which will not be translated nor included in the internationalization message dictionary.</param>
        /// <param name="errorCode">Specifies the SQLSTATE error identifier code for the condition.</param>
        /// <param name="errorDetailInternal">Supplies an optional "detail" message;
        /// this is to be used when there is additional information that seems inappropriate to put in the primary message.
        /// The message string will not be translated nor included in the internationalization message dictionary.</param>
        /// <param name="errorDetailLog">The same as <see cref="errorDetailInternal"/> except that this string goes only to the server log,
        /// never to the client.
        /// If both <see cref="errorDetailInternal"/> and <see cref="errorDetailLog"/> are used then one string goes to the client and the other to the log.
        /// This is useful for error details that are too security-sensitive or too bulky to include in the report sent to the client.</param>
        /// <param name="errorHint">Supplies an optional "hint" message;
        /// this is to be used when offering suggestions about how to fix the problem,
        /// as opposed to factual details about what went wrong.</param>
        /// <param name="errorPosition">Specifies the textual location of an error within a query string or function body.</param>
        /// <param name="errorDataType">Specifies a data type whose name and schema name should be included as auxiliary fields in the error report.</param>
        /// <returns></returns>
        public static PlClrUnreachableException? EReport(
            SeverityLevel level,
            string errorMessageInternal,
            PostgreSqlErrorCode? errorCode = null,
            string? errorDetailInternal = null,
            string? errorDetailLog = null,
            string? errorHint = null,
            int? errorPosition = null,
            uint? errorDataType = null)
        {
            if (errorMessageInternal == null)
                throw new ArgumentNullException(nameof(errorMessageInternal));

            _ereportDelegate!(
                (int)level,
                Marshal.ToPtrPalloc(errorMessageInternal),
                Marshal.ToPtrPalloc((int?)errorCode),
                Marshal.ToPtrPalloc(errorDetailInternal),
                Marshal.ToPtrPalloc(errorDetailLog),
                Marshal.ToPtrPalloc(errorHint),
                Marshal.ToPtrPalloc(errorPosition),
                Marshal.ToPtrPalloc(errorDataType)
                );

            return level >= SeverityLevel.Error ? new PlClrUnreachableException() : null;
        }
    }
}
