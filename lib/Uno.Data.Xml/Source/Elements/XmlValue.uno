using Uno.Collections;
using Uno.IO;
using Uno.Text;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlValue
    {
        public XmlValue()
        { }

        public XmlValue(string value)
        {
            Value = value;
        }

        public XmlValue(int value)
        {
            Value = value;
        }

        public XmlValue(long value)
        {
            Value = value;
        }

        public XmlValue(float value)
        {
            Value = value;
        }

        public XmlValue(double value)
        {
            Value = value;
        }

        public XmlValue(bool value)
        {
            Value = value;
        }

        public object Value { get; set; }

        public void Set(string value)
        {
            Value = value;
        }

        public void Set(int value)
        {
            Value = value;
        }

        public void Set(long value)
        {
            Value = value;
        }

        public void Set(float value)
        {
            Value = value;
        }

        public void Set(double value)
        {
            Value = value;
        }

        public void Set(bool value)
        {
            Value = value;
        }

        public string AsString()
        {
            if (Value is string)
            {
                return Value as string;
            }
            return Value == null ? null : Value.ToString();
        }

        public int AsInt()
        {
            if (Value is int)
            {
                return (int)Value;
            }
            return int.Parse(AsString());
        }

        public long AsLong()
        {
            if (Value is long)
            {
                return (long)Value;
            }
            return long.Parse(AsString());
        }

        public float AsFloat()
        {
            if (Value is float)
            {
                return (float)Value;
            }
            return float.Parse(AsString());
        }

        public double AsDouble()
        {
            if (Value is double)
            {
                return (double)Value;
            }
            return double.Parse(AsString());
        }

        public bool AsBool()
        {
            if (Value is bool)
            {
                return (bool)Value;
            }
            return bool.Parse(AsString());
        }

        public XmlValueType Type
        {
            get
            {
                if (Value is string)
                {
                    //needs to be refactored when TryParse methods are implemented
                    if (TryParseValue(BoolParseInternal))
                        return XmlValueType.Bool;

                    if (TryParseValue(IntParseInternal))
                        return XmlValueType.Int;

                    if (TryParseValue(LongParseInternal))
                        return XmlValueType.Long;

                    if (TryParseValue(FloatParseInternal))
                        return XmlValueType.Float;

                    if (TryParseValue(DoubleParseInternal))
                        return XmlValueType.Double;
                }
                else
                {
                    if (Value is bool)
                        return XmlValueType.Bool;
                    if (Value is int)
                        return XmlValueType.Int;
                    if (Value is long)
                        return XmlValueType.Long;
                    if (Value is float)
                        return XmlValueType.Float;
                    if (Value is double)
                        return XmlValueType.Double;
                }
                return XmlValueType.String;
            }
        }

        public static bool operator==(XmlValue xt, string value) { return xt.AsString() == value; }
        public static bool operator!=(XmlValue xt, string value) { return !(xt.AsString() == value); }

        public static bool operator==(XmlValue xt, int value) { return xt.AsInt() == value; }
        public static bool operator!=(XmlValue xt, int value) { return !(xt.AsInt() == value); }

        public static bool operator==(XmlValue xt, long value) { return xt.AsLong() == value; }
        public static bool operator!=(XmlValue xt, long value) { return !(xt.AsLong() == value); }

        public static bool operator==(XmlValue xt, float value) { return xt.AsFloat() == value; }
        public static bool operator!=(XmlValue xt, float value) { return !(xt.AsFloat() == value); }

        public static bool operator==(XmlValue xt, double value) { return xt.AsDouble() == value; }
        public static bool operator!=(XmlValue xt, double value) { return !(xt.AsDouble() == value); }

        public static bool operator==(XmlValue xt, bool value) { return xt.AsBool() == value; }
        public static bool operator!=(XmlValue xt, bool value) { return !(xt.AsBool() == value); }

        private bool TryParseValue(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void BoolParseInternal()
        {
            bool.Parse(AsString());
        }

        private void IntParseInternal()
        {
            int.Parse(AsString());
        }

        private void LongParseInternal()
        {
            long.Parse(AsString());
        }

        private void FloatParseInternal()
        {
            float.Parse(AsString());
        }

        private void DoubleParseInternal()
        {
            double.Parse(AsString());
        }
    }
}
