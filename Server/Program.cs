﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    internal class Program {
        static void Main(string[] args) {
            ServiceHost host = new ServiceHost(typeof(Service.GameService));
            host.Open();

            Console.WriteLine("Server is running...");
            Console.ReadLine();
        }
    }
}
