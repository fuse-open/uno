using System;
using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver : LogObject
    {
        readonly Dictionary<NamescopeKey, object> _scopeMembers = new Dictionary<NamescopeKey, object>();
        readonly Dictionary<NamescopeKey, object> _typeMembers = new Dictionary<NamescopeKey, object>();
        readonly ListDictionary<NamescopeKey, NamespaceUsings> _usings = new ListDictionary<NamescopeKey, NamespaceUsings>();
        readonly Dictionary<Tuple<DataType, DataType>, IReadOnlyList<Cast>> _casts = new Dictionary<Tuple<DataType, DataType>, IReadOnlyList<Cast>>();
        readonly Dictionary<Tuple<DataType, DataType, string>, IReadOnlyList<Operator>> _binOps = new Dictionary<Tuple<DataType, DataType, string>, IReadOnlyList<Operator>>();
        readonly Dictionary<Tuple<DataType, string>, IReadOnlyList<Operator>> _unOps = new Dictionary<Tuple<DataType, string>, IReadOnlyList<Operator>>();
        readonly Dictionary<DataType, int> _sizeOfs = new Dictionary<DataType, int>();
        readonly Compiler _compiler;

        public NameResolver(Compiler compiler)
            : base(compiler)
        {
            _compiler = compiler;
        }

        public Block GetBlock(PartialExpression p, AstExpression e)
        {
            switch (p.ExpressionType)
            {
                case PartialExpressionType.Block:
                    return ((PartialBlock) p).Block;
                case PartialExpressionType.Type:
                    var cls = ((PartialType) p).Type as ClassType;
                    if (cls != null)
                        return cls.Block;
                    break;
            }

            if (!p.IsInvalid)
                Log.Error(e.Source, ErrorCode.E3101, "Expression is <" + p.ExpressionType + "> but is used as a block.");

            return Block.Invalid;
        }

        public Block GetBlock(Namescope scope, AstExpression e)
        {
            return GetBlock(ResolveExpression(scope, e, null), e);
        }

        public Namespace GetNamespace(Namespace parent, string name)
        {
            var key = new NamescopeKey(parent, name);

            object result;
            if (!_scopeMembers.TryGetValue(key, out result))
            {
                result = new Namespace(parent, name);
                _scopeMembers[key] = result;
                parent.Namespaces.Add((Namespace) result);
            }

            return (Namespace)result;
        }

        internal void ClearCache()
        {
            _scopeMembers.Clear();
        }

        public void AddUsings(List<AstILNode> nodes, ICollection<Namespace> parentNamespaces, ICollection<DataType> parentTypes)
        {
            foreach (var node in nodes)
            {
                var key = new NamescopeKey(node.IL, node.Ast.Name.Source.FullPath, node.Ast.Name.Source.Part);
                var result = new NamespaceUsings(node.Ast.Name.Source.Line, parentNamespaces, parentTypes);
                _usings.Add(key, result);

                foreach (var directive in node.Ast.Usings)
                {
                    var partial = ResolveExpression(node.IL, directive.Expression, null);

                    if (partial.IsInvalid)
                        continue;

                    switch (partial.ExpressionType)
                    {
                        case PartialExpressionType.Type:
                        {
                            var dt = ((PartialType) partial).Type;

                            switch (dt.TypeType)
                            {
                                case TypeType.Class:
                                case TypeType.Struct:
                                    break;

                                default:
                                    Log.Error(directive.Expression.Source, ErrorCode.E0000, "Invalid 'using' on " + dt.TypeType.ToString().ToLower());
                                    continue;
                            }

                            if (dt.IsFlattenedDefinition)
                            {
                                Log.Error(directive.Expression.Source, ErrorCode.E0000, "Invalid 'using' on a generic type definition");
                                continue;
                            }

                            result.Types.Add(dt);
                            break;
                        }
                        case PartialExpressionType.Namespace:
                        {
                            var ns = ((PartialNamespace) partial).Namespace;

                            for (Namescope parent = node.IL;
                                 parent != null;
                                 parent = parent.Parent)
                                if (ns == parent)
                                    Log.Error(directive.Expression.Source, ErrorCode.E0000, "Invalid 'using' on a parent namespace");

                            result.Namespaces.Add(ns);
                            break;
                        }
                        default:
                        {
                            Log.Error(directive.Expression.Source, ErrorCode.E0000, "Invalid 'using' on " + partial.Quote());
                            break;
                        }
                    }
                }

                AddUsings(node.Nodes, result.Namespaces, result.Types);
            }
        }

        public int GetSizeOf(Source src, DataType dt)
        {
            int size, align;
            if (!_sizeOfs.TryGetValue(dt, out size) &&
                TryGetSizeOf(src, dt, dt.ToString(), true, out size, out align))
                _sizeOfs.Add(dt, size);
            return size;
        }

        bool TryGetSizeOf(Source src, DataType dt, string path, bool reportError, out int size, out int align)
        {
            switch (dt.TypeType)
            {
                case TypeType.Invalid:
                    align = 1;
                    size = 0;
                    return true;

                case TypeType.Enum:
                    return TryGetSizeOf(src, dt.Base, path, reportError, out size, out align);

                case TypeType.Struct:
                    {
                        switch (dt.BuiltinType)
                        {
                            case BuiltinType.Bool:
                                size = align = 1;
                                return true;
                            case BuiltinType.Byte:
                                size = align = 1;
                                return true;
                            case BuiltinType.Char:
                                size = align = 2;
                                return true;
                            case BuiltinType.Double:
                                size = align = 8;
                                return true;
                            case BuiltinType.Int:
                                size = align = 4;
                                return true;
                            case BuiltinType.Float:
                                size = align = 4;
                                return true;
                            case BuiltinType.Long:
                                size = align = 8;
                                return true;
                            case BuiltinType.SByte:
                                size = align = 1;
                                return true;
                            case BuiltinType.Short:
                                size = align = 2;
                                return true;
                            case BuiltinType.UInt:
                                size = align = 4;
                                return true;
                            case BuiltinType.ULong:
                                size = align = 8;
                                return true;
                            case BuiltinType.UShort:
                                size = align = 2;
                                return true;
                        }

                        var resultSum = true;
                        var fieldSizeSum = 0;
                        var fieldAlignMax = 1;

                        foreach (var f in dt.EnumerateFields())
                        {
                            int fieldSize;
                            int fieldAlign;
                            resultSum = TryGetSizeOf(src, f.ReturnType, path + "." + f.Name, reportError, out fieldSize, out fieldAlign) && resultSum;
                            int rem = fieldSizeSum % fieldAlign;

                            // Self-align field
                            if (rem > 0)
                                fieldSizeSum += fieldAlign - rem;

                            fieldSizeSum += fieldSize;
                            fieldAlignMax = Math.Max(fieldAlign, fieldAlignMax);
                        }

                        // Should not happen
                        if (resultSum && fieldSizeSum == 0 && dt.BuiltinType != 0)
                        {
                            resultSum = false;

                            if (reportError)
                                Log.Error(src, ErrorCode.I0000, "Cannot calculate size of " + path.Quote() + " (internal error)");
                        }
                        else
                        {
                            int rem = fieldSizeSum % fieldAlignMax;

                            // Self-align struct
                            if (rem > 0)
                                fieldSizeSum += fieldAlignMax - rem;
                        }

                        size = fieldSizeSum;
                        align = fieldAlignMax;
                        return resultSum;
                    }
            }

            if (reportError)
                Log.Error(src, ErrorCode.E0000, "Cannot calculate size of " + path.Quote() + " because it is not of struct or enum type");

            size = 0;
            align = 1;
            return false;
        }

        PartialExpression PartialError(Source src, ErrorCode code, string msg)
        {
            Log.Error(src, code, msg);
            return PartialExpression.Invalid;
        }
    }
}
