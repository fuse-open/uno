using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileVertexAttribExplicit(AstVertexAttribExplicit e)
        {
            var type = NameResolver.GetType(Namescope, e.Type);
            var args = CompileArgumentList(e.Arguments);

            if (args.Length < 4)
                return Error(e.Source, ErrorCode.E0000, "Minimum 4 arguments are required for 'vertex_attrib'");

            return new NewVertexAttrib(e.Source, type,
                CompileImplicitCast(args[0].Source, "Uno.Graphics.VertexAttributeType", args[0]),
                CompileImplicitCast(args[1].Source, "Uno.Graphics.VertexBuffer", args[1]),
                CompileImplicitCast(args[2].Source, Essentials.Int, args[2]),
                CompileImplicitCast(args[3].Source, Essentials.Int, args[3]),
                args.Length > 4
                    ? CompileImplicitCast(args[4].Source, "Uno.Graphics.IndexType", args[4])
                    : null,
                args.Length > 5
                    ? CompileImplicitCast(args[5].Source, "Uno.Graphics.IndexBuffer", args[5])
                    : null);
        }

        public Expression CompileVertexAttribImplicit(AstVertexAttribImplicit e)
        {
            var vertexBuffer = CompileExpression(e.VertexBuffer);
            DataType vertexDataType = null;

            if (vertexBuffer.ReturnType.IsArray)
                vertexDataType = vertexBuffer.ReturnType.ElementType;
            else if (!vertexBuffer.IsInvalid)
            {
                Log.Error(e.Source, ErrorCode.E2067, vertexBuffer.ReturnType.Quote() + " cannot be used to create vertex buffer because it is not an array type");
                vertexBuffer = Expression.Invalid;
            }

            Expression indexBuffer = null;
            DataType indexDataType = null;

            if (e.OptionalIndexBuffer != null)
            {
                indexBuffer = CompileExpression(e.OptionalIndexBuffer);

                if (indexBuffer.ReturnType.IsArray)
                    indexDataType = indexBuffer.ReturnType.ElementType;
                else if (!indexBuffer.IsInvalid)
                {
                    Log.Error(indexBuffer.Source, ErrorCode.E2069, indexBuffer.ReturnType.Quote() + " cannot be used to create index buffer because it is not an array type");
                    indexBuffer = Expression.Invalid;
                }
            }

            Expression vertexStride = null;
            Expression vertexAttributeType = null;

            if (vertexDataType != null)
            {
                switch (vertexDataType.BuiltinType)
                {
                    case BuiltinType.Float:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 4);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Float", "Uno.Graphics.VertexAttributeType");
                        VerifyNonNormalizedVertexAttribute(vertexBuffer.Source, e.Normalize);
                        break;
                    case BuiltinType.Float2:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 8);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Float2", "Uno.Graphics.VertexAttributeType");
                        VerifyNonNormalizedVertexAttribute(vertexBuffer.Source, e.Normalize);
                        break;
                    case BuiltinType.Float3:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 12);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Float3", "Uno.Graphics.VertexAttributeType");
                        VerifyNonNormalizedVertexAttribute(vertexBuffer.Source, e.Normalize);
                        break;
                    case BuiltinType.Float4:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 16);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Float4", "Uno.Graphics.VertexAttributeType");
                        VerifyNonNormalizedVertexAttribute(vertexBuffer.Source, e.Normalize);
                        break;
                    case BuiltinType.Short:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 2);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Short" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float;
                        break;
                    case BuiltinType.Short2:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 4);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Short2" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float2;
                        break;
                    case BuiltinType.Short4:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 8);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Short4" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float4;
                        break;
                    case BuiltinType.UShort:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 2);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.UShort" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float;
                        break;
                    case BuiltinType.UShort2:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 4);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.UShort2" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float2;
                        break;
                    case BuiltinType.UShort4:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 8);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.UShort4" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float4;
                        break;
                    case BuiltinType.SByte4:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 4);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.SByte4" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float4;
                        break;
                    case BuiltinType.Byte4:
                        vertexStride = new Constant(vertexBuffer.Source, Essentials.Int, 4);
                        vertexAttributeType = ILFactory.GetExpression(vertexBuffer.Source, "Uno.Graphics.VertexAttributeType.Byte4" + (e.Normalize ? "Normalized" : ""), "Uno.Graphics.VertexAttributeType");
                        vertexDataType = Essentials.Float4;
                        break;
                    default:
                        Log.Error(vertexBuffer.Source, ErrorCode.E0000, vertexDataType.Quote() + " is not a valid vertex attribute type");
                        vertexBuffer = Expression.Invalid;
                        break;
                }
            }

            Expression indexType = null;

            if (indexDataType != null)
            {
                switch (indexDataType.BuiltinType)
                {
                    case BuiltinType.Byte:
                        indexType = ILFactory.GetExpression(indexBuffer.Source, "Uno.Graphics.IndexType.Byte", "Uno.Graphics.IndexType");
                        break;
                    case BuiltinType.UShort:
                        indexType = ILFactory.GetExpression(indexBuffer.Source, "Uno.Graphics.IndexType.UShort", "Uno.Graphics.IndexType");
                        break;
                    case BuiltinType.UInt:
                        indexType = ILFactory.GetExpression(indexBuffer.Source, "Uno.Graphics.IndexType.UInt", "Uno.Graphics.IndexType");
                        break;
                    default:
                        Log.Error(indexBuffer.Source, ErrorCode.E0000, indexDataType.Quote() + " is not a valid index type");
                        indexBuffer = Expression.Invalid;
                        break;
                }
            }

            if (vertexBuffer.IsInvalid || indexBuffer != null && indexBuffer.IsInvalid)
                return Expression.Invalid;

            var vertexOffset = new Constant(vertexBuffer.Source, Essentials.Int, 0);
            return new NewVertexAttrib(e.Source, vertexDataType, vertexAttributeType, vertexBuffer, vertexStride, vertexOffset, indexType, indexBuffer);
        }

        void VerifyNonNormalizedVertexAttribute(Source src, bool normalized)
        {
            if (normalized)
                Log.Warning(src, ErrorCode.W0000, "'norm' does not have an effect on non-integral vertex attribute types");
        }
    }
}
