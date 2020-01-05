using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public enum SeverityLevel
    {
        Debug5 = 10,
        Debug4 = 11,
        Debug3 = 12,
        Debug2 = 13,
        Debug1 = 14,
        Log = 15,
        LogServerOnly = 16,
        CommError = LogServerOnly,
        Info = 17,
        Notice = 18,
        Warning = 19,
        Error = 20,
        Fatal = 21,
        Panic = 22
    }
}
