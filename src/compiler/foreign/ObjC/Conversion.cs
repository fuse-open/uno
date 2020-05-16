using System;
using System.Runtime.Remoting.Messaging;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Foreign.ObjC
{
    [Flags]
    enum ConversionFlags
    {
        None = 0,
        Boxed = 1 << 0, // Does the foreign type derive from Objective-C id?
        Identity = 1 << 1, // Is the conversion a no-op?
    }

    struct TypeConversion
    {
        public readonly DataType UnoType;
        public readonly string ForeignType;
        public readonly Func<string, string> FromUno; // @{UnoType} -> ForeignType
        public readonly Func<string, string> ToUno;   // ForeignType  -> @{UnoType}
        public readonly Func<string, string> Box;     // ForeignType -> id
        public readonly Func<string, string> Unbox;   // id -> ForeignType
        public readonly ConversionFlags ConversionFlags;

        public bool IsIdentity => ConversionFlags.HasFlag(ConversionFlags.Identity);
        public bool IsBoxed => ConversionFlags.HasFlag(ConversionFlags.Boxed);

        public TypeConversion(
            DataType unoType,
            string foreignType,
            Func<string, string> fromUno,
            Func<string, string> toUno,
            Func<string, string> box,
            Func<string, string> unbox,
            ConversionFlags conversionFlags)
        {
            UnoType = unoType;
            ForeignType = foreignType;
            FromUno = fromUno;
            ToUno = toUno;
            Box = box;
            Unbox = unbox;
            ConversionFlags = conversionFlags;
        }

        public static TypeConversion Identity(
            DataType unoType,
            string foreignType,
            Func<string, string> box,
            Func<string, string> unbox)
        {
            return new TypeConversion(
                unoType, foreignType,
                x => x, x => x,
                box, unbox,
                ConversionFlags.Identity);
        }

        public static TypeConversion Boxed(
            DataType unoType,
            string foreignType,
            Func<string, string> fromUno,
            Func<string, string> toUno)
        {
            return new TypeConversion(
                unoType, foreignType,
                fromUno, toUno,
                x => "(::id)" + x,
                x => "(" + foreignType + ")" + x,
                ConversionFlags.Boxed);
        }
    }

    struct FatConversion
    {
        public readonly string[] PreStatements;
        public string Expression;
        public readonly string[] PostStatements;

        public FatConversion(string[] preStatements, string expression, string[] postStatements)
        {
            PreStatements = preStatements;
            Expression = expression;
            PostStatements = postStatements;
        }

        public FatConversion(string expression)
        {
            PreStatements = new string[0];
            Expression = expression;
            PostStatements = new string[0];
        }
    }

    struct ParamTypeConversion
    {
        public readonly Parameter UnoParameter;
        public readonly string ForeignType;
        public readonly Func<string, FatConversion> FromUno; // @{UnoType} -> (setup statements, ForeignType, teardown statements)
        public readonly Func<string, FatConversion> ToUno;   // ForeignType  -> (setup statements, @{UnoType}, teardown statements)
        public readonly ConversionFlags ConversionFlags;

        public bool IsIdentity => ConversionFlags.HasFlag(ConversionFlags.Identity);

        public ParamTypeConversion(
            Parameter unoParameter,
            string foreignType,
            Func<string, FatConversion> fromUno,
            Func<string, FatConversion> toUno,
            ConversionFlags conversionFlags)
        {
            UnoParameter = unoParameter;
            ForeignType = foreignType;
            FromUno = fromUno;
            ToUno = toUno;
            ConversionFlags = conversionFlags;
        }

    }
}

