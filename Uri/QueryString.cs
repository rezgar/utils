using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Net;

namespace Rezgar.Utils.Uri
{
    public static class QueryString
    {
        public static IDictionary<string, string> GetQueryDictionary(this System.Uri uri)
        {
            return GetQueryDictionary(uri.Query);
        }
        public static IDictionary<string, string> GetQueryDictionary(string queryString)
        {
            return
                queryString.Replace("?", "")
                    .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);
        }
        public static string GetParameter(string queryString, string name)
        {
            var queryDictionary = GetQueryDictionary(queryString);
            string value;
            if (queryDictionary.TryGetValue(name, out value))
                return value;

            return null;
        }
        public static string SetParameter(string urlOrQuery, string name, string value)
        {
            if (urlOrQuery == null)
                return null;

            var nameEncoded = name != null ? WebUtility.UrlEncode(name) : name;
            var valueEncoded = value != null ? WebUtility.UrlEncode(value) : value;

            var queryStartIndex = urlOrQuery.IndexOf('?');
            var absoluteUrl = queryStartIndex == -1 ? urlOrQuery : urlOrQuery.Substring(0, queryStartIndex);

            var query = queryStartIndex == -1 ? String.Empty : urlOrQuery.Substring(queryStartIndex + 1);
            var pairs = query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            var sbResult = new StringBuilder(absoluteUrl);
            bool isFirstPair = true;
            bool isPairFound = false;

            foreach (var pair in pairs)
            {
                var parts = pair.Split(new[] { '=' }, 2);
                var varName = parts[0];
                var varValue = parts.Length == 1 ? String.Empty : parts[1];

                if (varName.Equals(nameEncoded, StringComparison.InvariantCultureIgnoreCase))
                {
                    isPairFound = true;
                    varValue = valueEncoded;
                }

                if (varValue != null) // empty is still valid
                {
                    sbResult.Append(isFirstPair ? "?" : "&");

                    if (string.IsNullOrEmpty(varValue))
                        sbResult.Append(varName);
                    else
                        sbResult.AppendFormat("{0}={1}", varName, varValue);

                    isFirstPair = false;
                }
            }

            if (!isPairFound && valueEncoded != null) // value empty is still valid
            {
                sbResult.Append(isFirstPair ? "?" : "&");
                if (string.IsNullOrEmpty(valueEncoded))
                    sbResult.Append(nameEncoded);
                else
                    sbResult.AppendFormat("{0}={1}", nameEncoded, valueEncoded);
            }

            return sbResult.ToString();
        }
    }
}
