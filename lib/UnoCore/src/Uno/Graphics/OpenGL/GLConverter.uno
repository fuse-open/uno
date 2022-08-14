using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics;

namespace Uno.Graphics.OpenGL
{
    public static extern(OPENGL) class GLConverter
    {
        public static GLFrontFaceDirection ToGLFrontFaceDirection(this PolygonWinding x)
        {
            switch (x)
            {
                case PolygonWinding.Clockwise:
                    return GLFrontFaceDirection.Cw;

                case PolygonWinding.CounterClockwise:
                    return GLFrontFaceDirection.Ccw;

                default:
                    throw new GLException("Unsupported polygon winding");
            }
        }

        public static PolygonWinding ToPolygonWinding(this GLFrontFaceDirection x)
        {
            switch (x)
            {
                case GLFrontFaceDirection.Cw:
                    return PolygonWinding.Clockwise;

                case GLFrontFaceDirection.Ccw:
                    return PolygonWinding.CounterClockwise;

                default:
                    throw new GLException("Unsupported polygon winding");
            }
        }

        public static GLCullFaceMode ToGLCullFaceMode(this PolygonFace x)
        {
            switch (x)
            {
                case PolygonFace.None:
                    return GLCullFaceMode.None;

                case PolygonFace.Front:
                    return GLCullFaceMode.Front;

                case PolygonFace.Back:
                    return GLCullFaceMode.Back;

                case PolygonFace.Both:
                    return GLCullFaceMode.FrontAndBack;

                default:
                    throw new GLException("Unsupported polygon face");
            }
        }

        public static PolygonFace ToPolygonFace(this GLCullFaceMode x)
        {
            switch (x)
            {
                case GLCullFaceMode.None:
                    return PolygonFace.None;

                case GLCullFaceMode.Front:
                    return PolygonFace.Front;

                case GLCullFaceMode.Back:
                    return PolygonFace.Back;

                case GLCullFaceMode.FrontAndBack:
                    return PolygonFace.Both;

                default:
                    throw new GLException("Unsupported polygon face");
            }
        }

        public static GLDepthFunction ToGLDepthFunction(this CompareFunc x)
        {
            switch (x)
            {
                case CompareFunc.Always:
                    return GLDepthFunction.Always;

                case CompareFunc.Less:
                    return GLDepthFunction.Less;

                case CompareFunc.LessOrEqual:
                    return GLDepthFunction.Lequal;

                case CompareFunc.Equal:
                    return GLDepthFunction.Equal;

                case CompareFunc.NotEqual:
                    return GLDepthFunction.Notequal;

                case CompareFunc.GreaterOrEqual:
                    return GLDepthFunction.Gequal;

                case CompareFunc.Greater:
                    return GLDepthFunction.Greater;

                case CompareFunc.Never:
                    return GLDepthFunction.Never;

                default:
                    throw new GLException("Unsupported compare func");
            }
        }

        public static CompareFunc ToCompareFunc(this GLDepthFunction x)
        {
            switch (x)
            {
                case GLDepthFunction.Always:
                    return CompareFunc.Always;

                case GLDepthFunction.Less:
                    return CompareFunc.Less;

                case GLDepthFunction.Lequal:
                    return CompareFunc.LessOrEqual;

                case GLDepthFunction.Equal:
                    return CompareFunc.Equal;

                case GLDepthFunction.Notequal:
                    return CompareFunc.NotEqual;

                case GLDepthFunction.Gequal:
                    return CompareFunc.GreaterOrEqual;

                case GLDepthFunction.Greater:
                    return CompareFunc.Greater;

                case GLDepthFunction.Never:
                    return CompareFunc.Never;

                default:
                    throw new GLException("Unsupported compare func");
            }
        }

        public static GLPrimitiveType ToGLPrimitiveType(this PrimitiveType x)
        {
            switch (x)
            {
                case PrimitiveType.Triangles:
                    return GLPrimitiveType.Triangles;

                case PrimitiveType.Lines:
                    return GLPrimitiveType.Lines;

                case PrimitiveType.Points:
                    return GLPrimitiveType.Points;

                case PrimitiveType.TriangleStrip:
                    return GLPrimitiveType.TriangleStrip;

                case PrimitiveType.LineStrip:
                    return GLPrimitiveType.LineStrip;

                default:
                    throw new GLException("Unsupported primitive type");
            }
        }

        public static PrimitiveType ToPrimitiveType(this GLPrimitiveType x)
        {
            switch (x)
            {
                case GLPrimitiveType.Triangles:
                    return PrimitiveType.Triangles;

                case GLPrimitiveType.Lines:
                    return PrimitiveType.Lines;

                case GLPrimitiveType.Points:
                    return PrimitiveType.Points;

                case GLPrimitiveType.TriangleStrip:
                    return PrimitiveType.TriangleStrip;

                case GLPrimitiveType.LineStrip:
                    return PrimitiveType.LineStrip;

                default:
                    throw new Exception("Unsupported primitive type");
            }
        }

        public static GLBlendingFactor ToGLBlendingFactor(this BlendOperand x)
        {
            switch (x)
            {
                case BlendOperand.Zero:
                    return GLBlendingFactor.Zero;

                case BlendOperand.One:
                    return GLBlendingFactor.One;

                case BlendOperand.SrcAlpha:
                    return GLBlendingFactor.SrcAlpha;

                case BlendOperand.OneMinusSrcAlpha:
                    return GLBlendingFactor.OneMinusSrcAlpha;

                case BlendOperand.SrcColor:
                    return GLBlendingFactor.SrcColor;

                case BlendOperand.OneMinusSrcColor:
                    return GLBlendingFactor.OneMinusSrcColor;

                case BlendOperand.DstAlpha:
                    return GLBlendingFactor.DstAlpha;

                case BlendOperand.OneMinusDstAlpha:
                    return GLBlendingFactor.OneMinusDstAlpha;

                case BlendOperand.DstColor:
                    return GLBlendingFactor.DstColor;

                case BlendOperand.OneMinusDstColor:
                    return GLBlendingFactor.OneMinusDstColor;

                default:
                    throw new GLException("Unsupported blend operand");
            }
        }

        public static BlendOperand ToBlendOperand(this GLBlendingFactor x)
        {
            switch (x)
            {
                case GLBlendingFactor.Zero:
                    return BlendOperand.Zero;

                case GLBlendingFactor.One:
                    return BlendOperand.One;

                case GLBlendingFactor.SrcAlpha:
                    return BlendOperand.SrcAlpha;

                case GLBlendingFactor.OneMinusSrcAlpha:
                    return BlendOperand.OneMinusSrcAlpha;

                case GLBlendingFactor.SrcColor:
                    return BlendOperand.SrcColor;

                case GLBlendingFactor.OneMinusSrcColor:
                    return BlendOperand.OneMinusSrcColor;

                case GLBlendingFactor.DstAlpha:
                    return BlendOperand.DstAlpha;

                case GLBlendingFactor.OneMinusDstAlpha:
                    return BlendOperand.OneMinusDstAlpha;

                case GLBlendingFactor.DstColor:
                    return BlendOperand.DstColor;

                case GLBlendingFactor.OneMinusDstColor:
                    return BlendOperand.OneMinusDstColor;

                default:
                    throw new GLException("Unsupported blend operand");
            }
        }

        public static GLBlendEquation ToGLBlendEquation(this BlendEquation x)
        {
            switch (x)
            {
                case BlendEquation.Add:
                    return GLBlendEquation.FuncAdd;

                case BlendEquation.Subtract:
                    return GLBlendEquation.FuncSubtract;

                case BlendEquation.ReverseSubtract:
                    return GLBlendEquation.FuncReverseSubtract;

                default:
                    throw new GLException("Unsupported blend equation");
            }
        }

        public static BlendEquation ToBlendEquation(this GLBlendEquation x)
        {
            switch (x)
            {
                case GLBlendEquation.FuncAdd:
                    return BlendEquation.Add;

                case GLBlendEquation.FuncSubtract:
                    return BlendEquation.Subtract;

                case GLBlendEquation.FuncReverseSubtract:
                    return BlendEquation.ReverseSubtract;

                default:
                    throw new GLException("Unsupported blend equation");
            }
        }

        public static GLBufferUsage ToGLBufferUsage(this BufferUsage x)
        {
            switch (x)
            {
                case BufferUsage.Immutable:
                    return GLBufferUsage.StaticDraw;

                case BufferUsage.Dynamic:
                    return GLBufferUsage.DynamicDraw;

                case BufferUsage.Stream:
                    return GLBufferUsage.StreamDraw;

                default:
                    throw new GLException("Unsupported buffer usage");
            }
        }

        public static GLIndexType ToGLIndexType(this IndexType x)
        {
            switch (x)
            {
                case IndexType.Byte:
                    return GLIndexType.UnsignedByte;

                case IndexType.UShort:
                    return GLIndexType.UnsignedShort;

                default:
                    throw new GLException("Unsupported index type");
            }
        }

        public static void ToGLVertexAttributeType(this VertexAttributeType x, out int componentCount, out GLDataType componentType, out bool normalized)
        {
            switch (x)
            {
                case VertexAttributeType.Float:
                    componentType = GLDataType.Float;
                    componentCount = 1;
                    normalized = false;
                    break;

                case VertexAttributeType.Float2:
                    componentType = GLDataType.Float;
                    componentCount = 2;
                    normalized = false;
                    break;

                case VertexAttributeType.Float3:
                    componentType = GLDataType.Float;
                    componentCount = 3;
                    normalized = false;
                    break;

                case VertexAttributeType.Float4:
                    componentType = GLDataType.Float;
                    componentCount = 4;
                    normalized = false;
                    break;

                case VertexAttributeType.Short:
                    componentType = GLDataType.Short;
                    componentCount = 1;
                    normalized = false;
                    break;

                case VertexAttributeType.ShortNormalized:
                    componentType = GLDataType.Short;
                    componentCount = 1;
                    normalized = true;
                    break;

                case VertexAttributeType.Short2:
                    componentType = GLDataType.Short;
                    componentCount = 2;
                    normalized = false;
                    break;

                case VertexAttributeType.Short2Normalized:
                    componentType = GLDataType.Short;
                    componentCount = 2;
                    normalized = true;
                    break;

                case VertexAttributeType.Short4:
                    componentType = GLDataType.Short;
                    componentCount = 4;
                    normalized = false;
                    break;

                case VertexAttributeType.Short4Normalized:
                    componentType = GLDataType.Short;
                    componentCount = 4;
                    normalized = true;
                    break;

                case VertexAttributeType.UShort:
                    componentType = GLDataType.UnsignedShort;
                    componentCount = 1;
                    normalized = false;
                    break;

                case VertexAttributeType.UShortNormalized:
                    componentType = GLDataType.UnsignedShort;
                    componentCount = 1;
                    normalized = true;
                    break;

                case VertexAttributeType.UShort2:
                    componentType = GLDataType.UnsignedShort;
                    componentCount = 2;
                    normalized = false;
                    break;

                case VertexAttributeType.UShort2Normalized:
                    componentType = GLDataType.UnsignedShort;
                    componentCount = 2;
                    normalized = true;
                    break;

                case VertexAttributeType.UShort4:
                    componentType = GLDataType.UnsignedShort;
                    componentCount = 4;
                    normalized = false;
                    break;

                case VertexAttributeType.UShort4Normalized:
                    componentType = GLDataType.UnsignedShort;
                    componentCount = 4;
                    normalized = true;
                    break;

                case VertexAttributeType.SByte4:
                    componentType = GLDataType.Byte;
                    componentCount = 4;
                    normalized = false;
                    break;

                case VertexAttributeType.SByte4Normalized:
                    componentType = GLDataType.Byte;
                    componentCount = 4;
                    normalized = true;
                    break;

                case VertexAttributeType.Byte4:
                    componentType = GLDataType.UnsignedByte;
                    componentCount = 4;
                    normalized = false;
                    break;

                case VertexAttributeType.Byte4Normalized:
                    componentType = GLDataType.UnsignedByte;
                    componentCount = 4;
                    normalized = true;
                    break;

                default:
                    throw new GLException("Unsupported vertex attribute type");
            }
        }
    }
}
