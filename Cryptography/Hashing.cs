using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Cryptography
{
    public static class Hashing
    {
        public static string Md5Hash(this string source, bool upperCase = true)
        {
            var cryptoServiceProvider = System.Security.Cryptography.MD5.Create();
            var hashBytes = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(source));

            var sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString(upperCase ? "X2" : "x2"));
            }

            return sb.ToString();
        }
    }
}
