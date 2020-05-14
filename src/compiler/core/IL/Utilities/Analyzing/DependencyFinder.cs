using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Utilities.Analyzing
{
    class DependencyFinder : Pass
    {
        readonly HashSet<IEntity> _dependencies = new HashSet<IEntity>();

        public static HashSet<IEntity> FindDependencies(Pass parent, Function function)
        {
            var p = new DependencyFinder(parent);
            function.Visit(p);
            return p._dependencies;
        }

        public static HashSet<IEntity> FindDependencies(Pass parent, Statement statement)
        {
            var p = new DependencyFinder(parent);
            statement.Visit(p);
            return p._dependencies;
        }

        DependencyFinder(Pass parent)
            : base(parent)
        {
        }

        void Add(DataType dt)
        {
            if (_dependencies.Contains(dt))
                return;

            _dependencies.Add(dt);

            if (dt.IsFlattenedParameterization)
                foreach (var a in dt.FlattenedArguments)
                    Add(a);
        }

        void Add(Method m)
        {
            if (_dependencies.Contains(m))
                return;

            _dependencies.Add(m);

            if (m.IsGenericParameterization)
                foreach (var a in m.GenericArguments)
                    Add(a);

            if (m.DeclaringMember != null)
                _dependencies.Add(m.DeclaringMember);

            Add(m.DeclaringType);
        }

        void Add(Field f)
        {
            if (_dependencies.Contains(f))
                return;

            _dependencies.Add(f);
            Add(f.DeclaringType);
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            Add(e.ReturnType);

            switch (e.ExpressionType)
            {
                case ExpressionType.CallMethod:
                {
                    var s = (CallMethod) e;
                    Add(s.Method);
                    break;
                }
                case ExpressionType.GetProperty:
                {
                    var s = (GetProperty) e;
                    Add(s.Property.GetMethod);
                    break;
                }
                case ExpressionType.SetProperty:
                {
                    var s = (SetProperty) e;
                    Add(s.Property.SetMethod);
                    break;
                }
                case ExpressionType.StoreField:
                {
                    var s = (StoreField) e;
                    Add(s.Field);
                    break;
                }
                case ExpressionType.LoadField:
                {
                    var s = (LoadField) e;
                    Add(s.Field);
                    break;
                }
                case ExpressionType.AddListener:
                {
                    var s = (AddListener) e;
                    Add(s.Event.AddMethod);
                    break;
                }
                case ExpressionType.RemoveListener:
                {
                    var s = (RemoveListener) e;
                    Add(s.Event.RemoveMethod);
                    break;
                }
                case ExpressionType.NewDelegate:
                {
                    var s = (NewDelegate) e;
                    Add(s.Method);
                    break;
                }
                case ExpressionType.IsOp:
                {
                    var s = (IsOp) e;
                    Add(s.TestType);
                    break;
                }
                case ExpressionType.TypeOf:
                {
                    var s = (TypeOf) e;
                    Add(s.Type);
                    break;
                }
            }
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.Throw:
                {
                    var s = (Throw) e;
                    Add(s.Exception.ReturnType);
                    break;
                }
                case StatementType.VariableDeclaration:
                {
                    var s = (VariableDeclaration) e;
                    Add(s.Variable.ValueType);
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    var s = (TryCatchFinally) e;
                    foreach (var c in s.CatchBlocks)
                        Add(c.Exception.ValueType);
                    break;
                }
            }
        }
    }
}
