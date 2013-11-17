using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace InitialRound.Common.Helpers
{
    public static class RandomHelper
    {
        public static long RandomLong()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[8];
                rng.GetBytes(bytes);

                long int64 = 0;

                foreach (byte b in bytes)
                {
                    int64 = (int64 << 8) | b;
                }

                return int64;
            }
        }
    }
}
