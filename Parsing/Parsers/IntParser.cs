using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Parsing.Parsers
{
    public class IntParser : IParser<int?> 
    {
        #region Implementation of IDataParser<int>

        public int? Parse(string value)
        {
            if (int.TryParse(value, out var result))
                return result;

            return null;
        }

        public string ToString(int? data)
        {
            return data?.ToString();
        }

        #endregion
    }
}
