using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.Classes
{
    public static class StringExtension
    {
        public static string GetLast(this string source, int tailLength)
        {
            return tailLength >= source.Length ? source : source.Substring(source.Length - tailLength);
        }
    }
}
