using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Common.Extensions
{
    public static class RandomExtensions
    {
        public static string NextString(this Random random, int size)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                builder.Append((char)('a' + random.Next(26)));
            }

            return builder.ToString();
        }
    }
}
