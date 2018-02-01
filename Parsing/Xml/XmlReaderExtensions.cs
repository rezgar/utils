using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace Rezgar.Utils.Parsing.Xml
{
    public static class XmlReaderExtensions
    {
        public static TType GetAttribute<TType>(this XmlReader reader, string name, TType defaultValue = default(TType))
        {
            string value = reader.GetAttribute(name);

            if (value == null)
                return defaultValue;

            var nullable = Nullable.GetUnderlyingType(typeof(TType));
            if ((nullable != null && nullable.GetTypeInfo().IsEnum))
            {
                return (TType)Enum.Parse(nullable, value, true);
            }

            if (typeof(TType).IsEnum)
            {
                return (TType)Enum.Parse(typeof(TType), value, true);
            }

            if (typeof(TType) == typeof(TimeSpan))
            {
                return (TType)(object)TimeSpan.Parse(value);
            }

            return (TType)ChangeType(value, typeof(TType));
        }

        public static void GetAttribute<TType>(this XmlReader reader, ref TType property, string name)
        {
            string value = reader.GetAttribute(name);

            if (value == null)
                return;

            var nullable = Nullable.GetUnderlyingType(typeof(TType));
            if ((nullable != null && nullable.GetTypeInfo().IsEnum))
            {
                property = (TType)Enum.Parse(nullable, value, true);
                return;
            }

            if (property is Enum)
            {
                property = (TType)Enum.Parse(typeof(TType), value, true);
                return;
            }

            if (property is TimeSpan)
            {
                property = (TType)(object)TimeSpan.Parse(value);
                return;
            }

            property = (TType)ChangeType(value, typeof(TType));
        }

        public static void ProcessChildren(this XmlReader reader, Action<string, XmlReader> nodeAction)
        {
            reader.ProcessChildren(
                (name, childReader) =>
                {
                    nodeAction(name, childReader);
                    return true;
                }).ToArray();
        }
        public static IEnumerable<TNodeResult> ProcessChildren<TNodeResult>(this XmlReader reader, Func<string, XmlReader, TNodeResult> nodeAction)
        {
            var parentName = reader.Name;
            while (!(reader.Name == parentName && (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement)) && reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                    continue;

                yield return nodeAction(reader.Name, reader);
            }
        }

        private static object ChangeType(object value, Type conversionType)
        {
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            } // end if

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if (conversionType.GetTypeInfo().IsGenericType &&
                conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
                // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
                // determine what the underlying type is
                // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
                // have a type--so just return null
                // Note: We only do this check if we're converting to a nullable type, since doing it outside
                // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
                // value is null and conversionType is a value type.
                if (value == null)
                {
                    return null;
                } // end if

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                conversionType = Nullable.GetUnderlyingType(conversionType);
            } // end if

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return Convert.ChangeType(value, conversionType);
        }
    }
}
