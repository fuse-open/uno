using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompilePartial(PartialExpression pe)
        {
             switch (pe.ExpressionType)
            {
                case PartialExpressionType.This:
                    return CompileThis(pe.Source);

                case PartialExpressionType.Value:
                {
                    var p = pe as PartialValue;
                    return p.Value;
                }
                case PartialExpressionType.ArrayElement:
                {
                    var p = pe as PartialArrayElement;
                    return new LoadElement(p.Source, p.Object, p.Index);
                }
                case PartialExpressionType.Variable:
                {
                    var p = pe as PartialVariable;

                    if (p.Variable.ValueType is FixedArrayType)
                        return new AddressOf(new LoadLocal(p.Source, p.Variable));

                    return new LoadLocal(p.Source, p.Variable);
                }
                case PartialExpressionType.Parameter:
                {
                    var p = pe as PartialParameter;

                    if (p.Parameter.Type is FixedArrayType)
                        return new AddressOf(new LoadArgument(p.Source, p.Function, p.Index), p.Parameter.Modifier == ParameterModifier.Const ? AddressType.Const : 0);

                    return new LoadArgument(p.Source, p.Function, p.Index);
                }
                case PartialExpressionType.Field:
                {
                    var p = pe as PartialField;

                    if (p.Field.ReturnType is FixedArrayType)
                        return new AddressOf(new LoadField(p.Source, p.Object, p.Field));

                    return new LoadField(p.Source, p.Object, p.Field);
                }
                case PartialExpressionType.Event:
                {
                    var e = pe as PartialEvent;

                    if (e.Event.ImplicitField == null)
                        return Error(e.Source, ErrorCode.E2056, "The event " + e.Event.Quote() + " has no implicit field and can only be used before '+=' or '-='");

                    return new LoadField(e.Source, e.Object, e.Event.ImplicitField);
                }
                case PartialExpressionType.Property:
                {
                    var p = pe as PartialProperty;

                    if (p.Property.GetMethod == null)
                        return Error(p.Source, ErrorCode.E2057, "The property " + p.Property.Quote() + " has no getter accessor");

                    var obj = p.Object;
                    if (obj != null)
                        Transforms.TryCreateReadonlyValueFieldIndirection(Namescope, ref obj);

                    return new GetProperty(p.Source, obj, p.Property);
                }
                case PartialExpressionType.Indexer:
                {
                    var p = pe as PartialIndexer;

                    if (p.Indexer.GetMethod == null)
                        return Error(p.Source, ErrorCode.E2058, "The indexer " + p.Indexer.Quote() + " has no getter accessor");

                    var obj = p.Object;

                    if (obj != null)
                        Transforms.TryCreateReadonlyValueFieldIndirection(Namescope, ref obj);

                    return new GetProperty(p.Source, obj, p.Indexer, p.Arguments);
                }
                case PartialExpressionType.MethodGroup:
                {
                    var p = pe as PartialMethodGroup;
                    var obj = p.Object;

                    if (obj != null)
                        Transforms.TryCreateReadonlyValueFieldIndirection(Namescope, ref obj);

                    return new MethodGroup(p.Source, obj, p.IsQualified, p.Methods);
                }
                case PartialExpressionType.ExtensionGroup:
                {
                    var p = pe as PartialExtensionGroup;
                    return new ExtensionGroup(p.Source, p.Object, p.Methods);
                }
                case PartialExpressionType.Namespace:
                {
                    var p = pe as PartialNamespace;
                    return Error(pe.Source, ErrorCode.E2059, "The namespace " + p.Namespace.Quote() + " is not a value");
                }
                case PartialExpressionType.Block:
                {
                    var p = pe as PartialBlock;
                    return Error(p.Source, ErrorCode.E2060, "The block " + p.Block.Quote() + " is not a value");
                }
                case PartialExpressionType.Type:
                {
                    var p = pe as PartialType;
                    return Error(p.Source, ErrorCode.E2061, "The type " + p.Type.Quote() + " is not a value");
                }
                default:
                {
                    return Error(pe.Source, ErrorCode.E2062, "<" + pe.ExpressionType + ">  is not a value");
                }
            }
        }
    }
}
