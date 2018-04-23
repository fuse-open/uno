using Uno.Collections;
using Uno;

namespace Uno.Data.Xml
{
    class XmlEncodingHelper
    {
        static Dictionary<string, XmlEncoding> _encodingDictionary;
        static Dictionary<string, XmlEncoding> EncodingDictionary
        {
            get
            {
                if (_encodingDictionary == null)
                {
                    FillEncodingDictionary();
                }
                return _encodingDictionary;
            }
        }

        public static XmlEncoding ConvertFromString(string encodingStr)
        {
            if (string.IsNullOrEmpty(encodingStr))
                return XmlEncoding.Auto;

            var encodingStrLowerCase = encodingStr.ToLower();
            if (!EncodingDictionary.ContainsKey(encodingStrLowerCase))
            {
                return XmlEncoding.Auto;
            }

            return EncodingDictionary[encodingStrLowerCase];
        }

        public static string ConvertToString(XmlEncoding encoding)
        {
            switch (encoding)
            {
                case XmlEncoding.Utf8:
                    return "UTF-8";
                case XmlEncoding.Utf16:
                    return "UTF-16";
                case XmlEncoding.Utf16_le:
                    return "UTF-16LE";
                case XmlEncoding.Utf16_be:
                    return "UTF-16BE";
                case XmlEncoding.Utf32:
                    return "UTF-32";
                case XmlEncoding.Utf32_le:
                    return "UTF-32LE";
                case XmlEncoding.Utf32_be:
                    return "UTF-32BE";
                case XmlEncoding.Latin1:
                    return "Latin1";
                case XmlEncoding.Wchar:
                    return "Wchar";
                default:
                    return null;
            }
        }

        private static void FillEncodingDictionary()
        {
            _encodingDictionary = new Dictionary<string, XmlEncoding>();
            _encodingDictionary.Add("utf-8", XmlEncoding.Utf8);
            _encodingDictionary.Add("utf-16be", XmlEncoding.Utf16_be);
            _encodingDictionary.Add("utf-16le", XmlEncoding.Utf16_le);
            _encodingDictionary.Add("utf-16", XmlEncoding.Utf16);
            _encodingDictionary.Add("utf-32be", XmlEncoding.Utf32_be);
            _encodingDictionary.Add("utf-32le", XmlEncoding.Utf32_le);
            _encodingDictionary.Add("utf-32", XmlEncoding.Utf32);
            _encodingDictionary.Add("latin1", XmlEncoding.Latin1);
            _encodingDictionary.Add("l1", XmlEncoding.Latin1);
            _encodingDictionary.Add("iso-ir-100", XmlEncoding.Latin1);
            _encodingDictionary.Add("csisolatin1", XmlEncoding.Latin1);
            _encodingDictionary.Add("cp819", XmlEncoding.Latin1);
            _encodingDictionary.Add("wchar", XmlEncoding.Wchar);
        }
    }
}
