using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    static class Printer {
        public static void Print(string message, [CallerMemberName]string callerName = "") {
#if (DEBUG)
            Console.WriteLine("{0}|{1}: {2}", DateTime.Now.ToString("hh:mm:ss"), callerName, message);
#endif
        }
    }
}
