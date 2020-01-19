using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public class PlClrUnreachableException : Exception
    {
        public PlClrUnreachableException() : base("This exception is thrown by PL/CLR in cases when the normal code flow is terminated by the server (i. e. elog() with error level >= ERROR). If you see it somewhere in the wild, something has gone wrong and this might indicate a bug.")
        {}
    }
}
