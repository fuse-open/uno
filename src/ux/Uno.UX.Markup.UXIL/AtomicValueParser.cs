using System;
using System.Linq;
using System.Collections.Generic;

namespace Uno.UX.Markup.UXIL
{
    public sealed class EnumValue : AtomicValue
    {
        readonly string _literalName;

        public Reflection.IEnum Enum { get; }

        public int LiteralIntValue { get; }

        public EnumValue(Reflection.IEnum dt, string literalName, FileSourceInfo srcInfo)
            : base(srcInfo)
        {
            Enum = dt;
            _literalName = literalName;

            var v = Enum.Literals.FirstOrDefault(x => x.Name == _literalName);

            if (v == null) throw new Exception("Invalid '" + dt.FullName + "' value: " + literalName);

            LiteralIntValue = v.Value;
        }

        public override string ToLiteral()
        {
            return Enum.FullName + "." + _literalName;
        }
    }

    // Hack - sometimes generic typed properties are used with reference properties
    // This sure isn't clean, but seems to work
    public sealed class ReferenceValue: AtomicValue
    {
        public NodeSource Value { get; }

        public ReferenceValue(FileSourceInfo src, NodeSource node): base(src)
        {
            Value = node;
        }

        public override string ToLiteral()
        {
            return Value.Node.Name;
        }
    }

    public sealed class GlobalReferenceValue: AtomicValue
    {
        internal Reflection.IDataType DataType { get; }

        internal string Identifier { get; }

        internal string ResolvedPath { get; set; }
        public AtomicValue ResolvedValue { get; set; }

        public GlobalReferenceValue(string identifier, Reflection.IDataType dt, FileSourceInfo src): base(src)
        {
            Identifier = identifier;
            DataType = dt;
        }

        public override string ToLiteral()
        {
            if (ResolvedValue != null)
            {
                return ResolvedValue.ToLiteral();
            }

            if (ResolvedPath == null) throw new Exception("GlobalReferenceValue not resolved: " + Identifier);
            return ResolvedPath;
        }
    }

    public partial class Compiler
    {
        static AtomicValue ParseArbitraryValue(string s, Reflection.IDataType dt, FileSourceInfo src)
        {
            if (s.StartsWith('\'') && s.EndsWith('\'')) return new String(s.Trim('\''), src);
            if (s.StartsWith('"') && s.EndsWith('"')) return new String(s.Trim('"'), src);
            if (s == "true") return new Bool(true, src);
            if (s == "false") return new Bool(false, src);
            var p = s.Split(',');
            if (p.Length > 1) return AtomicValueParser.ParseFloatVector<Single>(s, p.Length, src);
            if (s.Contains("#"))
            {
                return AtomicValueParser.ParseHexVector<float>(s, 4, src);
            }
            if (s.Contains("px") || s.Contains("%") || s.Contains("pt"))
            {
                if (p.Length == 2) return AtomicValueParser.ParseSize2(s, src);
                else return AtomicValueParser.ParseSize(s, src);
            }
            if (char.IsLetter(s[0]))
            {
                return new GlobalReferenceValue(s, dt, src);
            }

            return AtomicValueParser.ParseFloat<Double>(s, src);
        }

        public AtomicValue Parse(string value, Reflection.IDataType dt, FileSourceInfo src)
        {
            try
            {
                if (dt is Reflection.IEnum)
                {
                    return ParseEnum(value, (Reflection.IEnum)dt, src);
                }

                if (dt.FullName == "string")
                {
                    return new String(value, src);
                }

                if (dt.FullName == "Uno.UX.Selector")
                {
                    return new Selector(value, src);
                }

                if (dt.FullName == "bool")
                {
                    return new Bool(bool.Parse(value.ToLower()), src);
                }

                if (value.Length == 0)
                {
                    throw new Exception("Cannot parse empty string as " + dt.FullName);
                }

                if (char.IsLetter(value[0]))
                {
                    return new GlobalReferenceValue(value, dt, src);
                }

                switch (dt.FullName)
                {
                    case "sbyte": return Markup.AtomicValueParser.ParseInteger(value, x => sbyte.Parse(value), src);
                    case "sbyte2": return Markup.AtomicValueParser.ParseIntegerVector(value, 2, x => sbyte.Parse(value), src);
                    case "sbyte4": return Markup.AtomicValueParser.ParseIntegerVector(value, 4, x => sbyte.Parse(value), src);
                    case "byte": return Markup.AtomicValueParser.ParseInteger(value, x => byte.Parse(value), src);
                    case "byte2": return Markup.AtomicValueParser.ParseIntegerVector(value, 2, x => byte.Parse(value), src);
                    case "byte4": return Markup.AtomicValueParser.ParseIntegerVector(value, 4, x => byte.Parse(value), src);
                    case "short": return Markup.AtomicValueParser.ParseInteger(value, x => short.Parse(value), src);
                    case "short2": return Markup.AtomicValueParser.ParseIntegerVector(value, 2, x => short.Parse(value), src);
                    case "short4": return Markup.AtomicValueParser.ParseIntegerVector(value, 4, x => short.Parse(value), src);
                    case "ushort": return Markup.AtomicValueParser.ParseInteger(value, x => ushort.Parse(value), src);
                    case "ushort2": return Markup.AtomicValueParser.ParseIntegerVector(value, 2, x => ushort.Parse(value), src);
                    case "ushort4": return Markup.AtomicValueParser.ParseIntegerVector(value, 4, x => ushort.Parse(value), src);
                    case "int": return Markup.AtomicValueParser.ParseInteger(value, x => int.Parse(value), src);
                    case "int2": return Markup.AtomicValueParser.ParseIntegerVector(value, 2, int.Parse, src);
                    case "int3": return Markup.AtomicValueParser.ParseIntegerVector(value, 3, int.Parse, src);
                    case "int4": return Markup.AtomicValueParser.ParseIntegerVector(value, 4, int.Parse, src);
                    case "uint": return Markup.AtomicValueParser.ParseInteger(value, x => uint.Parse(value), src);
                    case "long": return Markup.AtomicValueParser.ParseInteger(value, x => long.Parse(value), src);
                    case "ulong": return Markup.AtomicValueParser.ParseInteger(value, x => ulong.Parse(value), src);
                    case "float": return Markup.AtomicValueParser.ParseFloat<Single>(value, src);
                    case "float2": return Markup.AtomicValueParser.ParseFloatVector<Single>(value, 2, src);
                    case "float3": return Markup.AtomicValueParser.ParseFloatVector<Single>(value, 3, src);
                    case "float4": return Markup.AtomicValueParser.ParseFloatVector<Single>(value, 4, src);
                    case "double": return Markup.AtomicValueParser.ParseFloat<Double>(value, src);
                    case "Uno.UX.Size": return Markup.AtomicValueParser.ParseSize(value, src);
                    case "Uno.UX.Size2": return Markup.AtomicValueParser.ParseSize2(value, src);
                }
            }
            catch (Exception e)
            {
                string msg = "Cannot parse '" + value + "' as '" + dt.FullName + "': '" + e.Message + "'";

                if (value.Length > 0 && char.IsLetter(value[0]))
                {
                    msg += ". Note that value types can not be referenced by name";
                }

                ReportError(src, msg);
                return null;
            }

            ReportError(src, "Cannot parse unrecognized atomic type: " + dt.FullName);
            return null;
        }

        static EnumValue ParseEnum(string s, Reflection.IEnum e, FileSourceInfo src)
        {
            // TODO: deal with magic encodings (bitflags etc)

            return new EnumValue(e, s, src);
        }

    }
}
