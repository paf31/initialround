using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace InitialRound.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct
        {
            return typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.GetValue(null).Equals(value))
                .SelectMany(f => f.GetCustomAttributes(false).OfType<DescriptionAttribute>())
                .Select(d => d.Description)
                .FirstOrDefault();
        }
    }
}
