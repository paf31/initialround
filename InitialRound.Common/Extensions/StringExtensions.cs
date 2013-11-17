using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Common.Extensions
{
    public static class StringExtensions
    {
        public static string[] Lines(this string s)
        {
            return s.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public static string Truncate(this string s, int len)
        {
            return s == null ? null : s.Substring(0, Math.Min(len, s.Length));
        }

        public static bool EqualsLenient(this string expected, string value)
        {
            if (expected == null)
            {
                return value == null;
            }

            if (value == null)
            {
                return false;
            }

            if (expected.Length > 0)
            {
                if (!char.IsWhiteSpace(expected[0]))
                {
                    value = value.TrimStart();
                }

                if (!char.IsWhiteSpace(expected[expected.Length - 1]))
                {
                    value = value.TrimEnd();
                }
            }

            return value.Equals(expected);
        }
    }
}
