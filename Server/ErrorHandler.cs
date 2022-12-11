using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    internal class ErrorHandler : IErrorHandler {
        public bool HandleError(Exception error) {
            Trace.TraceError($"EXCEPTION: {error.GetType().Name} -> {error.Message}");
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault) {
            var newEx = new FaultException($"EXCEPTION: {error.GetType().Name}");
            MessageFault msgFault = newEx.CreateMessageFault();
            fault = Message.CreateMessage(version, msgFault, newEx.Action);
        }
    }
}
