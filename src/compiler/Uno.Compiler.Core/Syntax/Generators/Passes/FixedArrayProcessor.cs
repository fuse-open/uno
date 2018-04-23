using System.Collections.Generic;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL;

namespace Uno.Compiler.Core.Syntax.Generators.Passes
{
    static class FixedArrayProcessor
    {
        class FixedArrayClassTransform : CompilerPass
        {
            readonly HashSet<object> TransformedVars = new HashSet<object>();

            public FixedArrayClassTransform(CompilerPass parent)
                : base(parent)
            {
            }

            public override void Begin(ref Statement e)
            {
                switch (e.StatementType)
                {
                    // TODO: DRY
                    case StatementType.VariableDeclaration:
                        {
                            var s = e as VariableDeclaration;
                            var var = s.Variable;
                            var fat = var.ValueType as FixedArrayType;
                            if (fat == null)
                                return;

                            var rat = TypeBuilder.GetArray(fat.ElementType);

                            var.ValueType = rat;
                            TransformedVars.Add(var);

                            var fa = var.OptionalValue as PlaceholderArray;

                            if (fa != null)
                            {
                                if (fa.OptionalInitializer != null)
                                    var.OptionalValue = new NewArray(fa.Source, rat, fa.OptionalInitializer);
                                else if (fa.ReturnType is FixedArrayType)
                                    var.OptionalValue = new NewArray(fa.Source, rat, (fa.ReturnType as FixedArrayType).OptionalSize);
                            }
                        }
                        break;

                    case StatementType.FixedArrayDeclaration:
                        {
                            var s = e as FixedArrayDeclaration;
                            var var = s.Variable;
                            var fat = (FixedArrayType) var.ValueType;
                            var rat = TypeBuilder.GetArray(fat.ElementType);

                            var.ValueType = rat;
                            TransformedVars.Add(var);

                            var fa = var.OptionalValue as PlaceholderArray;

                            if (fa != null)
                            {
                                if (fa.OptionalInitializer != null)
                                    var.OptionalValue = new NewArray(fa.Source, rat, fa.OptionalInitializer);
                                else if (fa.ReturnType is FixedArrayType)
                                    var.OptionalValue = new NewArray(fa.Source, rat, (fa.ReturnType as FixedArrayType).OptionalSize);
                            }
                        }
                        break;

                    case StatementType.Expression:
                        {
                            var es = e as Expression;

                            switch (es.ExpressionType)
                            {
                                case ExpressionType.StoreField:
                                    {
                                        var s = e as StoreField;
                                        var fat = s.Field.ReturnType as FixedArrayType;

                                        if (fat != null)
                                        {
                                            s.Field.ReturnType = TypeBuilder.GetArray(fat.ElementType);
                                            TransformedVars.Add(s.Field);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }

            public override void Begin(ref Expression e, ExpressionUsage u)
            {
                switch (e.ExpressionType)
                {
                    case ExpressionType.CapturedArgument:
                        {
                            var s = e as CapturedArgument;
                            e = new LoadArgument(s.Source, s.Function, s.ParameterIndex);
                        }
                        break;

                    case ExpressionType.CapturedLocal:
                        {
                            var s = e as CapturedLocal;
                            e = new LoadLocal(s.Source, s.Variable);
                        }
                        break;

                    case ExpressionType.AddressOf:
                        {
                            var s = e as AddressOf;

                            switch (s.Operand.ExpressionType)
                            {
                                case ExpressionType.LoadField:
                                    {
                                        var v = s.Operand as LoadField;

                                        if (TransformedVars.Contains(v.Field))
                                        {
                                            e = s.Operand;
                                            Begin(ref e, u);
                                        }
                                    }
                                    break;

                                case ExpressionType.LoadLocal:
                                    {
                                        var v = s.Operand as LoadLocal;

                                        if (TransformedVars.Contains(v.Variable))
                                        {
                                            e = s.Operand;
                                            Begin(ref e, u);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        class FixedArrayShaderTransform : Pass
        {
            public FixedArrayShaderTransform(CompilerPass parent)
                : base(parent)
            {
            }

            public override void Begin(ref Statement e)
            {

            }
        }

        public static void Process(CompilerPass parent, DataType dt, Scope initScope, HashSet<Scope> drawScopes)
        {
            var p = new FixedArrayClassTransform(parent);

            initScope.Visit(p);

            foreach (var s in drawScopes)
                s.Visit(p);
        }

        public static void Process(CompilerPass parent, Shader shader)
        {

        }
    }
}
