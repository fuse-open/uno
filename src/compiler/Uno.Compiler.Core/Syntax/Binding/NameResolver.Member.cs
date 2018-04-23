using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        Namescope TryGetMemberInternal(Namescope namescope, string name, int? typeParamCount)
        {
            switch (namescope.NamescopeType)
            {
                case NamescopeType.DataType:
                {
                    var dt = (DataType) namescope;

                    if (typeParamCount != null)
                    {
                        int pc = typeParamCount.Value;
                        foreach (var it in dt.NestedTypes)
                            if (it.IsGenericDefinition &&
                                it.GenericParameters.Length == pc &&
                                it.UnoName == name)
                                return it;
                        break;
                    }

                    foreach (var it in dt.NestedTypes)
                        if (it.UnoName == name)
                            return it;

                    if (dt.IsGenericDefinition)
                        foreach (var gt in dt.GenericParameters)
                            if (gt.UnoName == name)
                                return gt;

                    if (dt.IsGenericParameterization)
                        foreach (var gt in dt.GenericArguments)
                            if (gt.IsGenericParameter && gt.UnoName == name)
                                return gt;

                    if (dt.Block != null)
                        return TryGetMemberCached(dt.Block, name, typeParamCount) as Namescope;
                    break;
                }
                case NamescopeType.BlockBase:
                {
                    if (typeParamCount == null &&
                        namescope is Block)
                    {
                        var block = namescope as Block;
                        block.Populate();

                        foreach (var ib in block.NestedBlocks)
                            if (ib.UnoName == name)
                                return ib;
                    }
                    break;
                }
                case NamescopeType.Namespace:
                {
                    var ns = (Namespace) namescope;

                    if (typeParamCount != null)
                    {
                        int pc = typeParamCount.Value;
                        foreach (var dt in ns.Types)
                            if (dt.IsGenericDefinition &&
                                dt.GenericParameters.Length == pc &&
                                dt.UnoName == name)
                                return dt;
                        break;
                    }

                    foreach (var dt in ns.Types)
                        if (dt.UnoName == name)
                            return dt;
                    foreach (var block in ns.Blocks)
                        if (block.UnoName == name)
                            return block;
                    foreach (var child in ns.Namespaces)
                        if (child.UnoName == name)
                            return child;
                    break;
                }
            }

            return null;
        }

        object TryGetMemberCached(Namescope namescope, string name, int? typeParamCount)
        {
            var key = new NamescopeKey(namescope, name, typeParamCount ?? 0);

            object result;
            if (!_scopeMembers.TryGetValue(key, out result))
            {
                result = TryGetMemberInternal(namescope, name, typeParamCount);
                _scopeMembers[key] = result;
            }

            return result;
        }

        public PartialExpression TryResolveMember(Namescope namescope, AstIdentifier id, int? typeParamCount, AstExpression qualifier)
        {
            var result = TryGetMemberCached(namescope, id.Symbol, typeParamCount);

            if (result != null)
            {
                var gt = result as GenericParameterType;
                if (gt != null)
                    return qualifier == null
                        ? new PartialType(id.Source, gt)
                        : null;

                var dt = result as DataType;
                if (dt != null)
                    return dt.IsAccessibleFrom(id.Source)
                        ? new PartialType(id.Source, dt)
                        : null;

                var block = result as Block;
                if (block != null)
                    return block.IsAccessibleFrom(id.Source)
                        ? new PartialBlock(id.Source, block)
                        : null;

                var ns = result as Namespace;
                if (ns != null)
                    return ns.IsAccessibleFrom(id.Source)
                        ? new PartialNamespace(id.Source, ns)
                        : null;
            }

            var ndt = namescope as DataType;
            if (ndt != null && qualifier == null)
            {
                ndt.AssignBaseType();
                if (ndt.Base != null)
                    return TryResolveMember(ndt.Base, id, typeParamCount, null);
            }

            return null;
        }

        public PartialExpression TryResolveMemberRecursive(Namescope nameScope, AstIdentifier id, int? typeParamCount)
        {
            return TryResolveMember(nameScope, id, typeParamCount, null) ??
                (nameScope.IsRoot
                    ? null
                    : TryResolveMemberRecursive(nameScope.Parent, id, typeParamCount));
        }
    }
}
