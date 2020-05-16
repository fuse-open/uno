using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities;

namespace Uno.Compiler.Core.IL.Building.Types
{
    class FixedArrayTransform : CompilerPass
    {
        public FixedArrayTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Condition => !Backend.IsDefault;

        public override bool Begin()
        {
            new FixedArrayAddressOfRemover(this).Run();
            return true;
        }

        public override bool Begin(Shader s)
        {
            return false;
        }

        public override bool Begin(MetaProperty mp)
        {
            if (mp.ReturnType.IsFixedArray)
            {
                var fat = (FixedArrayType)mp.ReturnType;
                mp.ReturnType = TypeBuilder.GetArray(fat.ElementType);
            }

            return true;
        }

        public override bool Begin(DataType dt)
        {
            foreach (var m in dt.EnumerateFields())
            {
                var fat = m.ReturnType as FixedArrayType;

                if (fat != null)
                {
                    var pt = TypeBuilder.Parameterize(m.DeclaringType);
                    var rat = TypeBuilder.GetArray(fat.ElementType);

                    foreach (var ctor in m.DeclaringType.Constructors)
                        if (ctor.HasBody)
                            ctor.Body.Statements.Insert(0,
                                new StoreField(m.Source, new This(m.Source, pt).Address, m,
                                    new NewArray(m.Source, rat, fat.OptionalSize)));

                    m.ReturnType = rat;
                }
            }

            foreach (var m in dt.EnumerateFunctions())
            {
                foreach (var p in m.Parameters)
                {
                    var fat = p.Type as FixedArrayType;

                    if (fat != null)
                    {
                        p.Type = TypeBuilder.GetArray(fat.ElementType);
                        p.Modifier = 0;
                    }
                }
            }

            if (dt.IsGenericDefinition)
                foreach (var pt in dt.GenericParameterizations)
                    Begin(pt);

            return true;
        }

        public override void Begin(ref Statement e)
        {
            if (e is FixedArrayDeclaration)
            {
                var s = e as FixedArrayDeclaration;
                var var = s.Variable;
                var fat = (FixedArrayType)var.ValueType;
                var rat = TypeBuilder.GetArray(fat.ElementType);

                var.ValueType = rat;
                var.OptionalValue = s.OptionalInitializer != null
                    ? new NewArray(s.Source, rat, s.OptionalInitializer)
                    : new NewArray(s.Source, rat, fat.OptionalSize);

                e = new VariableDeclaration(var);
            }
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.GetProperty:
                {
                    var s = (GetProperty) e;
                    if (s.TryTransformGetFixedArrayLength(Log, ref e))
                        Begin(ref e, u);
                    break;
                }
            }
        }

        class FixedArrayAddressOfRemover : Pass
        {
            public FixedArrayAddressOfRemover(CompilerPass parent)
                : base(parent)
            {
            }

            public override bool Begin(Shader s)
            {
                return false;
            }

            public override void Begin(ref Expression e, ExpressionUsage u)
            {
                if (e is AddressOf && e.ReturnType is FixedArrayType)
                    e = e.ActualValue;
            }
        }
    }
}
