using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Parsing
{
    public interface IParser<TDataType>
    {
        TDataType Parse(string value);
        string ToString(TDataType data);
    }
}
