using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Core {
    public static class Random {
        private static int GetRandomNumber() {
            var randomGenerator = RandomNumberGenerator.Create();
            byte[] data = new byte[32];
            randomGenerator.GetBytes(data);
            return BitConverter.ToInt32(data, 0);
        }

        public static int Next(int min, int max) {
            return GetRandomNumber() % (max - min) + min;
        }

        public static int Next(int max) {
            return GetRandomNumber() % max;
        }
    }
}
