using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Parsing.Parsers
{
    using System.Globalization;

    public class DecimalParser : IParser<decimal?>
    {
        private readonly string _valueFormatTarget;

        public DecimalParser(string valueFormatTarget = "N")
        {
            _valueFormatTarget = valueFormatTarget;
        }

        #region Implementation of IDataParser<decimal?>

        public decimal? Parse(string value)
        {
            if (decimal.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                return result;

            return null;
        }

        public string ToString(decimal? data)
        {
            return data?.ToString(_valueFormatTarget, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
