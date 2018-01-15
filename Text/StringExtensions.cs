using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Rezgar.Utils.Text
{
    public static class StringExtensions
    {
        public static string ToASCII(this string s)
        {
            return String.Join("",
                 s.Normalize(NormalizationForm.FormD)
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }
    }
}
