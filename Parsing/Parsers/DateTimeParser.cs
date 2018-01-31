using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Parsing.Parsers
{
    using System.Globalization;

    public class DateTimeParser : IParser<DateTime?>
    {
        protected readonly string _valueFormatOriginal;
        protected readonly string _valueFormatTarget;

        public DateTimeParser(string valueFormatOriginal = null, string valueFormatTarget = "u")
        {
            _valueFormatOriginal = valueFormatOriginal;
            _valueFormatTarget = valueFormatTarget;
        }

        #region Implementation of IDataParser<DateTime?>

        public DateTime? Parse(string value)
        {
            DateTime result;
            if (_valueFormatOriginal != null && DateTime.TryParseExact(value, _valueFormatOriginal, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) { }
            else if (_valueFormatOriginal == null && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return result;

            return null;
        }

        public string ToString(DateTime? data)
        {
            return data?.ToString(_valueFormatTarget, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
