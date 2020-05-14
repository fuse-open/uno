using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Building.Functions.Lambdas
{
    class Closure
    {
        public readonly DataType Type;
        public readonly Dictionary<Variable, Field> VariableFields;
        public readonly Dictionary<Parameter, Field> ParameterFields;
        public readonly List<Statement> InitStatements;
        public readonly Expression Expression;
        public readonly Closure Parent;

        public Closure(DataType type, List<Statement> initStatements, Expression expression, Closure parent = null)
        {
            Type = type;
            InitStatements = initStatements;
            Expression = expression;
            Parent = parent;
            VariableFields = new Dictionary<Variable, Field>();
            ParameterFields = new Dictionary<Parameter, Field>();
        }

        // Outside lambda
        public Expression Load(Variable v)
        {
            Field field;
            if (VariableFields.TryGetValue(v, out field))
                return new LoadField(Source.Unknown, Expression, field);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for local " + v);

            return Parent.Load(v);
        }

        public Expression Load(Parameter p)
        {
            Field field;
            if (ParameterFields.TryGetValue(p, out field))
                return new LoadField(Source.Unknown, Expression, field);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for parameter " + p);

            return Parent.Load(p);
        }

        public Expression Store(Variable v, Expression value)
        {
            Field field;
            if (VariableFields.TryGetValue(v, out field))
                return new StoreField(Source.Unknown, Expression, field, value);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for local " + v);

            return Parent.Store(v, value);
        }

        public Expression Store(Parameter p, Expression value)
        {
            Field field;
            if (ParameterFields.TryGetValue(p, out field))
                return new StoreField(Source.Unknown, Expression, field, value);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for parameter " + p);

            return Parent.Store(p, value);
        }

        public Expression This()
        {
            // 'this' is the first field of the root closure object (by convention)
            return Parent != null
                ? Parent.This()
                : new LoadField(Source.Unknown, Expression, Type.Fields[0]);
        }

        public Expression StoreThis(Expression value)
        {
            // 'this' is the first field of the root closure object (by convention)
            return Parent != null
                ? Parent.StoreThis(value)
                : new StoreField(Source.Unknown, Expression, Type.Fields[0], value);
        }

        public Expression StoreParent(Expression value)
        {
            // 'this' is the first field of a non-root closure object (by convention)
            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: Storing parent of root closure");

            return new StoreField(Source.Unknown, Expression, Type.Fields[0], value);
        }

        // Inside lambda
        public Expression LoadInside(Variable v, Expression thisExpr = null)
        {
            thisExpr = thisExpr ?? new This(Source.Unknown, Type);

            Field field;
            if (VariableFields.TryGetValue(v, out field))
                return new LoadField(Source.Unknown, thisExpr, field);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for local " + v);

            return Parent.LoadInside(v, new LoadField(Source.Unknown, thisExpr, Type.Fields[0]));
        }

        public Expression LoadInside(Parameter p, Expression thisExpr = null)
        {
            thisExpr = thisExpr ?? new This(Source.Unknown, Type);

            Field field;
            if (ParameterFields.TryGetValue(p, out field))
                return new LoadField(Source.Unknown, thisExpr, field);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for parameter " + p);

            return Parent.LoadInside(p, new LoadField(Source.Unknown, thisExpr, Type.Fields[0]));
        }

        public Expression StoreInside(Variable v, Expression value, Expression thisExpr = null)
        {
            thisExpr = thisExpr ?? new This(Source.Unknown, Type);

            Field field;
            if (VariableFields.TryGetValue(v, out field))
                return new StoreField(Source.Unknown, thisExpr, field, value);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for local " + v);

            return Parent.StoreInside(v, value, new LoadField(Source.Unknown, thisExpr, Type.Fields[0]));
        }

        public Expression StoreInside(Parameter p, Expression value, Expression thisExpr = null)
        {
            thisExpr = thisExpr ?? new This(Source.Unknown, Type);

            Field field;
            if (ParameterFields.TryGetValue(p, out field))
                return new StoreField(Source.Unknown, thisExpr, field, value);

            if (Parent == null)
                throw new InvalidOperationException("ClosureConversionTransform: No matching closure field for parameter " + p);

            return Parent.StoreInside(p, value, new LoadField(Source.Unknown, thisExpr, Type.Fields[0]));
        }

        public Expression ThisInside()
        {
            return ThisInside(new This(Source.Unknown, Type));
        }

        public Expression ThisInside(Expression thisExpr)
        {
            var loadFirstField = new LoadField(Source.Unknown, thisExpr, Type.Fields[0]);

            if (Parent == null)
                return loadFirstField;
            else
                return Parent.ThisInside(loadFirstField);
        }

        public Expression StoreThisInside(Expression value)
        {
            return StoreThisInside(value, new This(Source.Unknown, Type));
        }

        public Expression StoreThisInside(Expression value, Expression thisExpr)
        {
            if (Parent == null)
                return new StoreField(Source.Unknown, thisExpr, Type.Fields[0], value);
            else
                return Parent.StoreThisInside(value, new LoadField(Source.Unknown, thisExpr, Type.Fields[0]));
        }
    }
}