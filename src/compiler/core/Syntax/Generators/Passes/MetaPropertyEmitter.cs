using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Generators.Passes
{
    class MetaPropertyEmitter : Pass
    {
        readonly ShaderGenerator Generator;
        readonly Dictionary<PlaceholderValue, StageValue> EmittedValues = new Dictionary<PlaceholderValue, StageValue>();
        readonly List<LoadLocal> LoadHistory = new List<LoadLocal>();
        readonly List<Variable> LocalQueue = new List<Variable>();

        Statement Parent;
        MetaStage CurrentMetaPropertyStage;

        MetaPropertyEmitter(ShaderGenerator generator)
            : base(generator.Compiler.Pass)
        {
            Generator = generator;
        }

        public override void Begin(ref Statement e)
        {
            Parent = e;
        }

        Expression EnqueueLocal(Source src, MetaProperty mp, MetaLocation loc, Expression e)
        {
            // TODO: Bugs without if test.
            if (e is PlaceholderArray)
            {
                var var = new Variable(src, null, Generator.CreateLocalName(mp, loc, e), e.ReturnType, VariableType.Default, new PlaceholderValue(mp, loc, e, CurrentMetaPropertyStage));
                LocalQueue.Add(var);

                var result = new LoadLocal(e.Source, var);
                LoadHistory.Add(result);

                if (result.ReturnType is FixedArrayType)
                    return new AddressOf(result);

                return result;
            }

            return e;
        }

        public override void Begin(ref Expression e, ExpressionUsage u = ExpressionUsage.Argument)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.PlaceholderReference:
                    {
                        var r = e as PlaceholderReference;
                        var s = EmittedValues[r.Value];

                        if (CurrentMetaPropertyStage != s.MinStage)
                        {
                            Generator.LocationStack.Add(r.Value.Location);
                            e = Generator.ProcessStageChange(s, s.MinStage, CurrentMetaPropertyStage, r.Value.Location, r.Value.Property).Value;
                            Begin(ref e, u);
                            Generator.LocationStack.RemoveLast();
                        }
                        else
                        {
                            e = s.Value;
                            Begin(ref e, u);
                        }
                    }

                    break;

                case ExpressionType.PlaceholderValue:
                    {
                        var mpv = e as PlaceholderValue;

                        e = mpv.Value;
                        Begin(ref e, u);

                        if (!(Parent is VariableDeclaration || Parent is FixedArrayDeclaration) &&
                            !Expressions.IsVariableOrGlobal(e))
                            e = EnqueueLocal(mpv.Source, mpv.Property, mpv.Location, e);
                    }

                    break;
            }

            Parent = e;
        }

        void FlushLocalQueue(Scope scope, ref int startIndex, ref int endIndex)
        {
            if (LocalQueue.Count > 0)
            {
                for (int i = LocalQueue.Count - 1; i >= 0; i--)
                {
                    var var = LocalQueue[i];
                    scope.Statements.Insert(startIndex, new VariableDeclaration(var));
                }

                var end = startIndex + LocalQueue.Count;
                LocalQueue.Clear();

                for (int i = startIndex; i < end; i++)
                    VisitVariableDeclaration(scope.Statements[i] as VariableDeclaration, scope, ref i, ref end);

                var diff = end - startIndex;
                startIndex += diff;
                endIndex += diff;
            }
        }

        void FlushLocalQueue(Scope scope)
        {
            int i = scope.Statements.Count;
            int endIndex = scope.Statements.Count;
            FlushLocalQueue(scope, ref i, ref endIndex);
        }

        bool VisitVariableDeclaration(VariableDeclaration s, Scope scope, ref int i, ref int endIndex)
        {
            var mpv = s.Variable.OptionalValue as PlaceholderValue;

            if (mpv != null)
            {
                Parent = s;

                Begin(ref mpv.Value);
                mpv.Value.Visit(this);
                End(ref mpv.Value);

                FlushLocalQueue(scope, ref i, ref endIndex);

                var var = s.Variable;
                var fa = mpv.Value as PlaceholderArray;

                if (fa != null && CurrentMetaPropertyStage > MetaStage.Volatile)
                {
                    var.OptionalValue = null;
                    scope.Statements[i] = new FixedArrayDeclaration(var, fa.OptionalInitializer);
                    EmittedValues.Add(mpv, new StageValue(new AddressOf(new LoadLocal(mpv.Source, var)), CurrentMetaPropertyStage, CurrentMetaPropertyStage));
                    return true;
                }
                else
                {
                    var.OptionalValue = mpv.Value;
                }

                var key = var.OptionalValue.ToString();
                var found = false;

                for (int j = 0; j < i; j++)
                {
                    if (scope.Statements[j] is VariableDeclaration)
                    {
                        var pvar = (scope.Statements[j] as VariableDeclaration).Variable;

                        if (pvar.ValueType.Equals(var.ValueType) && pvar.OptionalValue != null && pvar.OptionalValue.ToString() == key)
                        {
                            Expression value = new LoadLocal(s.Source, pvar);

                            if (value.ReturnType is FixedArrayType)
                                value = new AddressOf(value);

                            foreach (var load in LoadHistory)
                                if (load.Variable == var)
                                    load.Variable = pvar;

                            EmittedValues.Add(mpv, new StageValue(value, CurrentMetaPropertyStage, CurrentMetaPropertyStage));
                            scope.Statements.RemoveAt(i--);
                            found = true;
                            endIndex--;

                            break;
                        }
                    }
                }

                if (!found)
                {
                    Expression value = new LoadLocal(s.Source, var);

                    if (value.ReturnType is FixedArrayType)
                        value = new AddressOf(value);

                    EmittedValues.Add(mpv, new StageValue(value, CurrentMetaPropertyStage, CurrentMetaPropertyStage));
                }

                return true;
            }

            return false;
        }

        void Visit(MetaStage stage, Scope scope, int startIndex, int endIndex)
        {
            LoadHistory.Clear();
            CurrentMetaPropertyStage = stage;

            for (int i = startIndex; i < endIndex; i++)
            {
                var s = scope.Statements[i];
                var vd = s as VariableDeclaration;

                if (vd == null || !VisitVariableDeclaration(vd, scope, ref i, ref endIndex))
                {
                    s.Visit(this);
                    FlushLocalQueue(scope, ref i, ref endIndex);
                }
            }
        }

        public static void Emit(ShaderGenerator g)
        {
            var p = new MetaPropertyEmitter(g);

            p.Visit(MetaStage.ReadOnly, g.InitScope, g.InitStart, g.InitScope.Statements.Count);
            p.FlushLocalQueue(g.InitScope);

            p.Visit(MetaStage.Volatile, g.FrameScope, g.FrameStart, g.FrameScope.Statements.Count);
            g.DrawState.Visit(g.FrameScope, p);
            p.FlushLocalQueue(g.FrameScope);

            p.Visit(MetaStage.Vertex, g.VertexScope, 0, g.VertexScope.Statements.Count);
            g.DrawState.VisitVaryings(g.VertexScope, p);
            g.DrawState.VertexShader.VisitTerminals(g.VertexScope, p);
            p.FlushLocalQueue(g.VertexScope);

            p.Visit(MetaStage.Pixel, g.PixelScope, 0, g.PixelScope.Statements.Count);
            g.DrawState.PixelShader.VisitTerminals(g.PixelScope, p);
            p.FlushLocalQueue(g.PixelScope);
        }
    }
}
