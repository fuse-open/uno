using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public PartialExpression TryResolveLocalIdentifier(AstIdentifier id)
        {
            // Check if it is a local variable
            for (int i = VariableScopeStack.Count - 1; i >= 0; i--)
            {
                Variable var;
                if (VariableScopeStack[i].Variables.TryGetValue(id.Symbol, out var))
                    return var.IsConstant && var.OptionalValue != null
                        ? (PartialExpression) new PartialValue(var.OptionalValue)
                        :                     new PartialVariable(id.Source, var);
            }

            foreach (var lambda in Lambdas)
                for (int i = 0; i < lambda.Parameters.Length; i++)
                    if (lambda.Parameters[i].Name == id.Symbol)
                        return new PartialParameter(id.Source, lambda, i);

            for (int i = 0; i < Function.Parameters.Length; i++)
                if (Function.Parameters[i].Name == id.Symbol)
                    return new PartialParameter(id.Source, Function, i);

            return null;
        }

        public PartialExpression TryResolveCapturedLocalIdentifier(BlockBase block, AstIdentifier id)
        {
            var db = block.TryFindDrawBlockParent();
            if (db != null)
            {
                var em = db.Method;

                Variable var;
                if (db.CapturedLocals.TryGetValue(id.Symbol, out var))
                    return var.IsConstant && var.OptionalValue != null
                            ? new PartialValue(var.OptionalValue) :
                        var.ValueType is FixedArrayType 
                            ? new PartialValue(new AddressOf(new CapturedLocal(id.Source, em, var))) 
                            : new PartialValue(new CapturedLocal(id.Source, em, var));

                for (int i = 0; i < em.Parameters.Length; i++)
                    if (em.Parameters[i].Name == id.Symbol)
                        return em.Parameters[i].Type is FixedArrayType 
                            ? new PartialValue(new AddressOf(new CapturedArgument(id.Source, em, i), 
                                em.Parameters[i].Modifier == ParameterModifier.Const 
                                    ? AddressType.Const 
                                    : 0)) 
                            : new PartialValue(new CapturedArgument(id.Source, em, i));
            }

            return null;
        }

        public PartialExpression ResolveIdentifier(AstIdentifier id, int? typeParamCount)
        {
            if (typeParamCount == null)
            {
                var pi = TryResolveLocalIdentifier(id);
                if (pi != null)
                    return pi;
            }

            // Parent scope is resolved later
            Namescope parentScope;

            if (IsFunctionScope)
            {
                var dt = Namescope as DataType;

                // Should not happen
                if (dt == null)
                    return PartialError(id.Source, ErrorCode.I0000, "Namescope was a Function without DataType in ResolveIdentifier()");

                // Check if it as a member of the class
                var obj = Function != null && !Function.IsStatic
                    ? new This(id.Source, TypeBuilder.Parameterize(Function.DeclaringType)).Address
                    : null;

                var p = TryResolveTypeMember(dt, id, typeParamCount, null, obj);
                if (p != null)
                    return p;

                // Check if it is a member of the parent class
                parentScope = dt.Parent;
                var parentType = dt.ParentType;

                while (parentType != null)
                {
                    p = TryResolveTypeMember(parentType, id, typeParamCount, null, obj);
                    if (p != null)
                        return p;

                    parentScope = parentType.Parent;
                    parentType = parentType.ParentType;
                }
            }
            else
            {
                // Check if it is a meta property protected by a req statement
                if (MetaProperty != null && typeParamCount == null)
                {
                    foreach (var req in ReqStatements)
                    {
                        if (req is ReqProperty)
                        {
                            var rmp = req as ReqProperty;
                            if (rmp.PropertyName == id.Symbol && rmp.PropertyType != null)
                                return new PartialValue(new GetMetaProperty(id.Source, rmp.PropertyType, rmp.PropertyName));
                        }
                    }
                }

                var block = Namescope as BlockBase;

                if (block != null)
                {
                    if (typeParamCount == null)
                    {
                        var p = TryResolveCapturedLocalIdentifier(block, id);
                        var mp = NameResolver.TryGetMetaProperty(id.Source, block, block, id.Symbol, true);

                        if (p != null && mp != null)
                            Log.Error(id.Source, ErrorCode.E0000, id.Symbol.Quote() + " is an ambiguous match between meta property and captured local variable. Use 'meta " + id.Symbol + "' or 'local::" + id.Symbol + "' to disambiguate");
                        if (p != null)
                            return p;
                        if (mp != null)
                            return new PartialValue(new GetMetaProperty(id.Source, mp.ReturnType, mp.Name));
                    }

                    // Object context
                    for (var dt = block.TryFindTypeParent(); dt != null; dt = dt.ParentType)
                    {
                        var p = TryResolveTypeMember(dt, id, typeParamCount, null, new GetMetaObject(id.Source, TypeBuilder.Parameterize(dt)));
                        if (p != null)
                            return p;
                    }

                    // Static context
                    for (var dt = block.ParentType; dt != null; dt = dt.ParentType)
                    {
                        var p = TryResolveTypeMember(dt, id, typeParamCount, null, null);
                        if (p != null)
                            return p;
                    }
                }

                parentScope = Namescope;
            }

            if (parentScope != null)
            {
                // Check if it is a member of the parent namespace
                var p = NameResolver.TryResolveMemberRecursive(parentScope, id, typeParamCount);
                if (p != null)
                    return p;

                // Check if it is a member in a namespace referenced from a using-directive
                p = NameResolver.TryResolveUsingNamespace(parentScope, id, typeParamCount);
                if (p != null)
                    return p;

                // Check if it is a static method in a class referenced from a using static-directive
                p = NameResolver.TryResolveUsingType(parentScope, id, typeParamCount);
                if (p != null)
                    return p;
            }

            return PartialError(id.Source, ErrorCode.E3102, this.GetUnresolvedIdentifierError(id, typeParamCount));
        }
    }
}
