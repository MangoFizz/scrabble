using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core {
    public class Log {
        private static string CurrentTime() {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        public static void Info(string message) {
            Trace.TraceInformation($"[{CurrentTime()}] INFO: {message}");
        }

        public static void Error(string message) {
            Trace.TraceError($"[{CurrentTime()}] ERROR: {message}");
        }

        public static void Warning(string message) {
            Trace.TraceWarning($"[{CurrentTime()}] WARNING: {message}");
        }
    }
}
