using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public enum PostgreSqlErrorCode : int
    {
        /// <summary>
        /// Successful completion
        /// <para>Class 00 - Successful Completion<br/>
        /// Category: Success
        /// </para>
        /// </summary>
        // ReSharper disable ShiftExpressionZeroLeftOperand
        SuccessfulCompletion = (('0' - '0') & 0x3F) + ((('0' - '0') & 0x3F) << 6) + ((('0' - '0') & 0x3F) << 12) + ((('0' - '0') & 0x3F) << 18) + ((('0' - '0') & 0x3F) << 24),
        // ReSharper restore ShiftExpressionZeroLeftOperand
    }
}
