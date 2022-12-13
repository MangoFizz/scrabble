using Core;
using System;
using System.ServiceModel;

namespace Server {
    internal class Program {
        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) => {
                Log.Error($"Unhandled exception: {eventArgs.ExceptionObject}");
            };

            while(true) {
                try {
                    var service = new Service.GameService();
                    using(var host = new ServiceHost(service)) {
                        host.Open();
                        Log.Info("Service started");

                        ConsoleCloseEvent.Register(() => {
                            Log.Info("Service is shutting down...");
                            service.Shutdown();
                            Log.Info("Aborting service host... ");
                            host.Abort();
                        });

                        Console.WriteLine("Press any key to stop.");
                        Console.ReadKey();
                        
                        Log.Info("Service is shutting down...");
                        service.Shutdown();
                        Log.Info("Stopping service host... ");
                        host.Close();
                        
                        break;
                    }
                }
                catch(Exception ex) {
                    Log.Error($"Exception: {ex.GetType().Name} -> {ex.Message}");
                    Log.Info("Retrying in 5 seconds...");
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}
