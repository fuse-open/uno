using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL;

namespace Uno.Compiler.Core.Syntax.Generators.Passes
{
    class ScopeProcessor : CompilerPass
    {
        struct Argument
        {
            public readonly Source Source;
            public readonly StageValue Value;

            public Argument(Source src, StageValue val)
            {
                Source = src;
                Value = val;
            }
        }

        readonly ShaderGenerator Generator;
        readonly MetaProperty Property;
        readonly MetaLocation Location;

        readonly Method Method;
        readonly List<Argument> Arguments = new List<Argument>();

        MetaStage MinStage = MetaStage.Const;
        MetaStage MaxStage = MetaStage.Max;

        ScopeProcessor(ShaderGenerator g, Scope scope)
            : base(g.Compiler.Pass)
        {
            Generator = g;
            Location = g.LocationStack.Last();
            Property = g.GetProperty(Location);
            Method = new Method(Property.Source, g.Path.DrawBlock.Method.DeclaringType,
                null, Modifiers.Private | Modifiers.Static | Modifiers.Generated, 
                g.CreateFieldName(Property, Location), Property.ReturnType, 
                new Parameter[0]);

            Method.SetBody(scope.CopyNullable(new CopyState(Method)));
        }

        void SetMinStage(Source src, MetaStage min)
        {
            if (min > MinStage)
                MinStage = min;

            if (MinStage > MaxStage)
                Log.Error(src, ErrorCode.E5007, "Cannot access " + MinStage.Quote() + " symbol from stage " + MaxStage.Quote());
        }

        void SetMaxStage(Source src, MetaStage max)
        {
            if (max < MaxStage)
                MaxStage = max;

            if (MaxStage < MinStage)
                Log.Error(src, ErrorCode.E5008, "Cannot access " + MaxStage.Quote() + " symbol from stage " + MinStage.Quote());
        }

        Expression AddArgument(Source src, StageValue v)
        {
            var key = v.Value.ToString();

            for (int i = 0; i < Arguments.Count; i++)
                if (Arguments[i].Value.Value.ToString() == key)
                    return new PlaceholderArgument(src, Arguments[i].Value.Value.ReturnType, i);

            Arguments.Add(new Argument(src, v));
            return new PlaceholderArgument(src, v.Value.ReturnType, Arguments.Count - 1);
        }

        void OnObject(ref Expression e, ref Expression obj)
        {
            if (obj != null)
            {
                switch (obj.ExpressionType)
                {
                    case ExpressionType.Base:
                    case ExpressionType.This:
                    case ExpressionType.GetMetaObject:
                    case ExpressionType.CapturedArgument:
                    case ExpressionType.CapturedLocal:
                        var r = Generator.ProcessValue(obj);
                        obj = r.Value;
                        e = AddArgument(e.Source, new StageValue(e, r.MinStage, r.MaxStage));
                        break;

                    case ExpressionType.Swizzle:
                        OnObject(ref e, ref (obj as Swizzle).Object);
                        break;
                    case ExpressionType.LoadField:
                        OnObject(ref e, ref (obj as LoadField).Object);
                        break;
                    case ExpressionType.GetProperty:
                        OnObject(ref e, ref (obj as GetProperty).Object);
                        break;
                    case ExpressionType.CallMethod:
                        OnObject(ref e, ref (obj as CallMethod).Object);
                        break;

                    case ExpressionType.NewVertexAttrib:
                    case ExpressionType.NewPixelSampler:
                        Begin(ref obj, ExpressionUsage.Object);
                        break;
                }
            }
        }

        class NoLocalAccessOrLValueValidator : Pass
        {
            readonly string OperatorName;
            bool HasError;

            NoLocalAccessOrLValueValidator(Pass parent, string operatorName)
                : base(parent)
            {
                OperatorName = operatorName;
            }

            public override void Begin(ref Expression e, ExpressionUsage u)
            {
                switch (e.ExpressionType)
                {
                    case ExpressionType.LoadLocal:
                    case ExpressionType.LoadArgument:
                        Log.Error(e.Source, ErrorCode.E0000, "Local variable access are not allowed as part of a '" + OperatorName + "()' expression inside a meta property scope");
                        HasError = true;
                        break;

                    case ExpressionType.StoreElement:
                    case ExpressionType.StoreField:
                    case ExpressionType.StoreArgument:
                    case ExpressionType.StoreLocal:
                    case ExpressionType.SetProperty:
                        Log.Error(e.Source, ErrorCode.E0000, "Not allowed to use assign operators as part of a '" + OperatorName + "()' expression inside a meta property scope");
                        HasError = true;
                        break;
                }
            }

            public static bool Run(Pass parent, string operatorName, Expression e, ExpressionUsage u)
            {
                var p = new NoLocalAccessOrLValueValidator(parent, operatorName);
                p.VisitNullable(ref e, u);
                return !p.HasError;
            }
        }

        class ArgumentResolver : Pass
        {
            readonly Dictionary<int, Expression> Map;

            public ArgumentResolver(Pass parent, Dictionary<int, Expression> map)
                : base(parent)
            {
                Map = map;
            }

            public override void Begin(ref Expression e, ExpressionUsage u)
            {
                switch (e.ExpressionType)
                {
                    case ExpressionType.PlaceholderArgument:
                        e = Map[((PlaceholderArgument) e).Index];
                        break;
                }
            }
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Base:
                case ExpressionType.This:
                case ExpressionType.GetMetaObject:
                case ExpressionType.GetMetaProperty:
                case ExpressionType.CapturedArgument:
                case ExpressionType.CapturedLocal:
                    {
                        var s = Generator.ProcessValue(e);

                        if (Expressions.IsVariableOrGlobal(s.Value))
                        {
                            e = s.Value;
                            SetMinStage(e.Source, s.MinStage);
                            SetMaxStage(e.Source, s.MaxStage);
                        }
                        else
                        {
                            e = AddArgument(e.Source, s);
                        }
                    }
                    break;

                case ExpressionType.NewVertexAttrib:
                case ExpressionType.NewPixelSampler:
                    {
                        if (!NoLocalAccessOrLValueValidator.Run(this, e.ExpressionType == ExpressionType.NewVertexAttrib ? "vertex_attrib" : "pixel_sampler", e, u))
                        {
                            e = Expression.Invalid;
                            break;
                        }

                        var s = Generator.ProcessValue(e);
                        SetMinStage(e.Source, s.MinStage);
                        SetMaxStage(e.Source, s.MaxStage);
                        e = s.Value;
                    }
                    break;

                case ExpressionType.Swizzle:
                    OnObject(ref e, ref (e as Swizzle).Object);
                    break;
                case ExpressionType.LoadField:
                    OnObject(ref e, ref (e as LoadField).Object);
                    break;
                case ExpressionType.GetProperty:
                    OnObject(ref e, ref (e as GetProperty).Object);
                    break;
                case ExpressionType.CallMethod:
                    OnObject(ref e, ref (e as CallMethod).Object);
                    break;
            }
        }

        StageValue Process()
        {
            Method.Body.Visit(this);

            var min = MinStage;
            var max = MaxStage;

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (Arguments[i].Value.MinStage > min)
                    min = Arguments[i].Value.MinStage;

                if (Arguments[i].Value.MaxStage < max)
                    max = Arguments[i].Value.MaxStage;

                if (Arguments[i].Value.MinStage > MaxStage)
                    Log.Error(Arguments[i].Source, ErrorCode.E5009, "Cannot access " + min.Quote() + " symbol from stage " + MaxStage.Quote());
            }

            if (max < min)
                max = min;

            var pl = new List<Parameter>();
            var al = new List<Expression>();

            var map = new Dictionary<int, Expression>();

            for (int i = 0; i < Arguments.Count; i++)
            {
                var e = Generator.ProcessStage(Arguments[i].Value, min, max).Value;

                if (min >= MetaStage.Vertex && Expressions.IsVariableOrGlobal(e))
                {
                    map.Add(i, e);
                    continue;
                }

                pl.Add(new Parameter(Arguments[i].Source, AttributeList.Empty,
                    e.ReturnType is FixedArrayType ? ParameterModifier.Ref : 0, e.ReturnType,
                    Generator.CreateShaderName(Property, Location, Arguments[i].Value.Value), null));

                al.Add(e);
                map.Add(i, new LoadArgument(e.Source, Method, pl.Count - 1));
            }

            Method.SetParameters(pl.ToArray());

            if (min <= MetaStage.Volatile)
            {
                // See if an equivalent method is already generated on this class scope
                foreach (var m in Generator.Path.DrawBlock.Method.DeclaringType.Methods)
                    if (m.Source == Method.Source &&
                        m.Modifiers == Method.Modifiers &&
                        m.ReturnType == Method.ReturnType &&
                        m.CompareParameters(Method))
                        return new StageValue(new CallMethod(m.Source, m.IsStatic ? null : new This(m.Source, m.DeclaringType), m, al.ToArray()), min, max);

                Generator.Path.DrawBlock.Method.DeclaringType.Methods.Add(Method);
            }
            else
            {
                Method.SetName(Generator.CreateShaderName(Property, Location));
            }

            Method.Body.Visit(new ArgumentResolver(this, map));
            return new StageValue(new CallMethod(Method.Source, Method.IsStatic ? null : new This(Method.Source, Method.DeclaringType), Method, al.ToArray()), min, max);
        }

        public static StageValue Process(ShaderGenerator g, Scope scope)
        {
            return new ScopeProcessor(g, scope).Process();
        }
    }
}
