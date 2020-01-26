using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public static class Globals
    {
        // Static members
        // We have to cheat about nullability here, since the initialization can't happen in a static constructor\
        // but we'll make sure that everything gets initialized in the Setup function
        public static ServerFunctions BackendFunctions { get; internal set; } = null!;

    }
}
