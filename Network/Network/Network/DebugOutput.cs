using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public static class DebugOutput
    {
        public static Action<string> DebugAction = Console.WriteLine;
    }
}
