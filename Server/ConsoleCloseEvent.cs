using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    public class ConsoleCloseEvent {
        static ConsoleEventDelegate ConsoleEventHandler;
        
        private delegate bool ConsoleEventDelegate(int eventType);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        /// <summary>
        /// Register a callback for the event. 
        /// The execution of the function MUST last less than 5 seconds,
        /// after that windows will force program exit.
        /// </summary>
        /// <param name="callback">Function to be executed before the console window is closed</param>
        public static void Register(Action callback) {
            ConsoleEventHandler = new ConsoleEventDelegate(eventType => {
                if(eventType == 2) {
                    callback();
                }
                return false;
            });
            SetConsoleCtrlHandler(ConsoleEventHandler, true);
        }
    }
}
