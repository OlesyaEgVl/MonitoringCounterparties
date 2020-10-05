using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ContragentAnalyse.Extension
{
    public static class StringExtension
    {
        public static string TrimSpaces(this string str)
        {
            return str.Replace("  ", string.Empty);
        }
    }
}
