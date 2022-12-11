using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    internal class Program {
        static void Main(string[] args) {
            ServiceHost host = new ServiceHost(typeof(Service.GameService));

            IErrorHandler errorHandler = new ErrorHandler();
            foreach(ChannelDispatcherBase channelDispatcherBase in host.ChannelDispatchers) {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                if(channelDispatcher != null) {
                    channelDispatcher.ErrorHandlers.Add(errorHandler);
                }
            }

            host.Open();

            Console.WriteLine("Server is running...");
            Console.ReadLine();
        }
    }
}
