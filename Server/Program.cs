using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    internal class Program {
        static void Main(string[] args) {
            ServiceHost authManagerHost = new ServiceHost(typeof(Service.AuthManager));
            authManagerHost.Open();

            ServiceHost gameChatHost = new ServiceHost(typeof(Service.GameChat));
            gameChatHost.Open();

            Console.WriteLine("Server is running");
            Console.ReadLine();
        }
    }
}
