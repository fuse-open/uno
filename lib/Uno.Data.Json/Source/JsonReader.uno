using Uno;
using Uno.IO;
using Uno.Collections;

namespace Uno.Data.Json
{
    public class JsonReader
    {
        Value _value;

        JsonReader(Value value)
        {
            _value = value;
        }

        public JsonReader this[string key]
        {
            get
            {
                var obj = _value as Object;

                if (obj == null || !(obj.ContainsKey(key)))
                    return null;

                return new JsonReader(obj[key]);
            }
        }

        public JsonReader this[int index]
        {
            get
            {
                var array = _value as Array;
                if (array == null)
                    throw new Exception("Json node is not an array");

                return new JsonReader(array[index]);
            }
        }

        public string[] Keys
        {
            get
            {
                var obj = _value as Object;
                if (obj == null)
                    return new string[0];

                return obj.Keys;
            }
        }

        public static JsonReader Parse(string json)
        {
            return new JsonReader(Parser.Parse(new Uno.IO.StringReader(json)));
        }

        public JsonDataType JsonDataType
        {
            get
            {
                if (_value is Number) return JsonDataType.Number;
                if (_value is Boolean) return JsonDataType.Boolean;
                if (_value is String) return JsonDataType.String;
                if (_value is Array) return JsonDataType.Array;
                if (_value is Object) return JsonDataType.Object;
                if (_value is Null) return JsonDataType.Null;

                return JsonDataType.Error;
            }
        }

        public int Count
        {
            get
            {
                var obj = _value as Object;
                if (obj != null)
                    return obj.Count;

                var array = _value as Array;
                if (array != null)
                    return array.Count;

                return 0;
            }
        }

        public bool HasKey(string key)
        {
            var obj = _value as Object;
            if (obj == null)
                return false;

            return obj.ContainsKey(key);
        }

        public string AsString()
        {
            return ((String)_value).Value;
        }

        public double AsNumber()
        {
            return ((Number)_value).Value;
        }

        public bool AsBool()
        {
            return ((Boolean)_value).Value;
        }

        public static explicit operator string(JsonReader value)
        {
            return value.AsString();
        }

        public static explicit operator float(JsonReader value)
        {
            return (float)value.AsNumber();
        }

        public static explicit operator bool(JsonReader value)
        {
            return value.AsBool();
        }

        public static explicit operator int(JsonReader value)
        {
            return (int)value.AsNumber();
        }
    }
}
