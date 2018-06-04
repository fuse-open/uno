using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Validation
{
    public class ILAnalyzer : CompilerPass
    {
        // TODO: Report warning on unused fields
        private readonly Dictionary<Variable, VariableUsage> _locals = new Dictionary<Variable, VariableUsage>();
        private readonly Dictionary<Field, VariableUsage> _fields = new Dictionary<Field, VariableUsage>();
        private readonly Method _objectEquals;
        private readonly Method _objectGetHashCode;

        public ILAnalyzer(CompilerPass parent)
            : base(parent)
        {
            _objectGetHashCode = ILFactory.GetEntity("object.GetHashCode()") as Method;
            _objectEquals = ILFactory.GetEntity("object.Equals(object)") as Method;
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.IsOp:
                    {
                        var s = (IsOp) e;

                        if (!s.TestType.IsRelatedTo(s.Operand.ReturnType) &&
                            !s.TestType.IsInterface &&
                            !s.TestType.IsGenericParameter &&
                            !s.Operand.ReturnType.IsInterface)
                            Log.Warning(s.Source, ErrorCode.W0000, "Expression is never of type " + s.TestType.Quote());
                    }
                    break;
                case ExpressionType.StoreLocal:
                    {
                        var store = (StoreLocal) e;
                        EnsureExsits(store.Variable);
                        _locals[store.Variable] |= VariableUsage.Stored;
                    }
                    break;
                case ExpressionType.LoadLocal:
                    {
                        var load = (LoadLocal) e;
                        EnsureExsits(load.Variable);
                        _locals[load.Variable] |= VariableUsage.Loaded;
                    }
                    break;
                case ExpressionType.LoadField:
                    {
                        var load = (LoadField) e;
                        EnsureExsits(load.Field);
                        _fields[load.Field] |= VariableUsage.Loaded;
                    }
                    break;
                case ExpressionType.StoreField:
                    {
                        var store = (StoreField) e;
                        EnsureExsits(store.Field);
                        _fields[store.Field] |= VariableUsage.Stored;
                    }
                    break;
            }
        }

        public override bool Begin(DataType dt)
        {
            if (dt.IsEnum || dt.CanLink || dt.Package.IsVerified)
                return false;

            if (dt.IsAbstract)
                foreach (var ctor in dt.Constructors)
                    if (ctor.IsPublic)
                        Log.Warning3(ctor.Source, ErrorCode.W3005, "'public' constructor in 'abstract' class -- change to 'protected'");

            foreach (var member in dt.EnumerateMembers())
                WarnForHiddenMember(dt, member, member.Modifiers);

            foreach (var innerType in dt.NestedTypes)
                WarnForHiddenMember(dt, innerType, innerType.Modifiers);

            foreach (var field in dt.Fields)
                EnsureExsits(field);

            if (HasEqualityOrInequalityOperator(dt) && !OverridesEquals(dt) && !dt.IsEnum)
                Log.Warning(dt.Source, ErrorCode.W0000, dt.Quote() + " defines operator == or operator != but does not override object.Equals(object o)");

            if (HasEqualityOrInequalityOperator(dt) && !OverridesGetHashCode(dt) && !dt.IsEnum)
                Log.Warning(dt.Source, ErrorCode.W0000, dt.Quote() + " defines operator == or operator != but does not override object.GetHashCode()");

            if (OverridesEquals(dt) && !OverridesGetHashCode(dt))
                Log.Warning(dt.Source, ErrorCode.W0000, dt.Quote() + " overrides object.Equals(object o) but does not override object.GetHashCode()");

            foreach (var m in dt.Methods)
                if (m.IsGenericDefinition && dt.IsFlattenedDefinition)
                    foreach (var mp in m.GenericParameters)
                        for (var pt = dt; pt != null; pt = pt.ParentType)
                            if (pt.IsGenericDefinition)
                                foreach (var pp in pt.GenericParameters)
                                    if (mp.Name == pp.Name)
                                        Log.Warning(mp.Source, ErrorCode.W0000, "Type parameter " + mp.Name.Quote() + " has the same name as the type parameter from outer type " + pt.Quote());

            if (dt.IsClass && dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                Log.Warning(dt.Source, ErrorCode.W0000, "[TargetSpecificType] class is not supported -- use struct instead for proper boxing semantics");

            return true;
        }

        private static bool HasEqualityOrInequalityOperator(DataType dt)
        {
            return dt.Operators.Any(op => (op.Name == "op_Equality" || op.Name == "op_Inequality") && 
                    op.Parameters.Length == 2 && op.Parameters[0].Type == dt && op.Parameters[1].Type == dt);
        }

        private bool OverridesGetHashCode(DataType dt)
        {
            return dt.Methods.Any(m => m.NameAndParameterList == "GetHashCode()" && m.IsOverridingMethod(_objectGetHashCode));
        }

        private bool OverridesEquals(DataType dt)
        {
            return dt.Methods.Any(m => m.NameAndParameterList == "Equals(object)" && m.IsOverridingMethod(_objectEquals));
        }

        private void WarnForHiddenMember(DataType parentType, Entity hidingMember, Modifiers modifiers)
        {
            var hiddenMember = TryFindHiddenMember(hidingMember, parentType.Base);

            if (hiddenMember != null && !hiddenMember.IsPrivate)
            {
                if (!modifiers.HasFlag(Modifiers.New))
                    Log.Warning(hidingMember.Source, ErrorCode.W0000, hidingMember.Quote() + " hides inherited member " + hiddenMember.Quote() + " -- use the 'new' modifier if hiding is intentional");
            }
            else
            {
                if (modifiers.HasFlag(Modifiers.New))
                    Log.Warning(hidingMember.Source, ErrorCode.W0000, hidingMember.Quote() + " specifies 'new', but does not hide any inherited members.");
            }
        }

        private Member TryFindHiddenMember(Entity hidingMember, DataType baseType)
        {
            while (baseType != null)
            {
                foreach (var field in baseType.Fields)
                    if (field.Name == hidingMember.Name && !field.IsPrivate)
                        return field;

                foreach (var literal in baseType.Literals)
                    if (literal.Name == hidingMember.Name && !literal.IsPrivate)
                        return literal;

                foreach (var property in baseType.Properties)
                {
                    if (property.Name == hidingMember.Name && !property.IsPrivate)
                    {
                        var hidingProperty = hidingMember as Property;

                        if (hidingProperty != null)
                        {
                            if (hidingProperty.OverriddenProperty == property ||
                                !hidingProperty.CompareParameters(property))
                                return null;
                        }

                        return property;
                    }
                }

                foreach (var method in baseType.Methods)
                {
                    if (method.Name == hidingMember.Name && !method.IsPrivate)
                    {
                        var hidingMethod = hidingMember as Method;

                        if (hidingMethod != null)
                        {
                            if (hidingMethod.OverriddenMethod != null &&
                                hidingMethod.OverriddenMethod.MasterDefinition == method.MasterDefinition)
                                return null;

                            if (hidingMethod.IsGenericDefinition &&
                                method.IsGenericDefinition &&
                                hidingMethod.Parameters.Length == method.Parameters.Length &&
                                hidingMethod.GenericParameters.Length == method.GenericParameters.Length)
                            {
                                var parameterizedMethod = TypeBuilder.Parameterize(method.Source, method, hidingMethod.GenericParameters);

                                if (hidingMethod.CompareParameters(parameterizedMethod))
                                    return method;
                            }

                            if (!hidingMethod.CompareParameters(method))
                                return null;
                        }

                        return method;
                    }
                }

                foreach (var @event in baseType.Events)
                {
                    if (@event.Name == hidingMember.Name && !@event.IsPrivate)
                    {
                        var hidingEvent = hidingMember as Event;

                        if (hidingEvent != null)
                        {
                            if (hidingEvent.OverriddenEvent == @event)
                                return null;
                        }

                        return @event;
                    }
                }

                baseType = baseType.Base;
            }

            return null;
        }

        public override void OnApply(Apply apply)
        {
            var dt = apply.Block.TryFindTypeParent();

            if (dt != null)
            {
                // TODO: Give some warnings
            }
        }

        public override void Begin(ref Statement statement)
        {
            var declaration = statement as VariableDeclaration;
            if (declaration != null)
            {
                for (var var = declaration.Variable; var != null; var = var.Next)
                {
                    EnsureExsits(var);
                    if (var.OptionalValue != null)
                        _locals[var] |= VariableUsage.Stored;
                }
            }
        }

        private void EnsureExsits(Variable variable)
        {
            if (!_locals.ContainsKey(variable))
                _locals.Add(variable, 0);
        }

        private void EnsureExsits(Field field)
        {
            if (!_fields.ContainsKey(field))
                _fields.Add(field, 0);
        }

        public override void End()
        {
            foreach (var variable in _locals)
            {
                if (variable.Key.IsExtern)
                    continue;

                switch (variable.Value)
                {
                    case 0:
                        Log.Warning(variable.Key.Source, ErrorCode.W0000,
                            "The variable " + variable.Key.Quote() + " is declared but never used");
                        break;
                    case VariableUsage.Stored:
                        if (variable.Key.IsConstant ||
                            variable.Key.Function.DeclaringType.Attributes.Any(x => x.Constructor.DeclaringType.FullName == "Uno.Compiler.UxGeneratedAttribute"))
                            break;
                        Log.Warning(variable.Key.Source, ErrorCode.W0000,
                            "The variable " + variable.Key.Quote() + " is assigned but its value is never used");
                        break;
                }
            }
            //This does not work, since declaring a field results in an Expression.LoadField
            //foreach (var field in _fields)
            //{
            //    switch (field.Value)
            //    {
            //        case 0:
            //            Log.ReportWarning(field.Key.Source, ErrorCode.W0000,
            //                "The field " + field.Key.Quoted() + " is declared but never used");
            //            break;
            //        case VariableUsage.Stored:
            //            Log.ReportWarning(field.Key.Source, ErrorCode.W0000,
            //                "The field " + field.Key.Quoted() + " is assigned but its value is never used");
            //            break;
            //        case VariableUsage.Loaded:
            //            Log.ReportWarning(field.Key.Source, ErrorCode.W0000,
            //                "Field " + field.Key.Quoted() + " is never assigned to, and will always have its default value");
            //            break;
            //    }
            //}
        }
    }
}
