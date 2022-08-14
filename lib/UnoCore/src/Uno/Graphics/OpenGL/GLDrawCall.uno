using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;
using Uno.Graphics;

namespace Uno.Graphics.OpenGL
{
    public extern(OPENGL) struct GLDrawCall
    {
        const GLTextureTarget VideoTextureTarget = defined(ANDROID)
            ? GLTextureTarget.TextureExternalOES
            : GLTextureTarget.Texture2D;

        // static: Must only be used from one thread
        static List<int> _boundAttributes;
        static int _currentTextureUnit;

        string[] _constValues;
        bool _compiledProgramDirty;
        GLCompiledProgram _compiledProgram;

        GLBlendEquation _blendEqRgb;
        GLBlendEquation _blendEqAlpha;
        GLBlendingFactor _blendSrcRgb;
        GLBlendingFactor _blendSrcAlpha;
        GLBlendingFactor _blendDstRgb;
        GLBlendingFactor _blendDstAlpha;
        GLDepthFunction _depthFunc;
        GLPrimitiveType _primitiveType;
        GLCullFaceMode _cullFace;
        GLFrontFaceDirection _frontFace;

        public GLProgram Program
        {
            get;
            private set;
        }

        public bool BlendEnabled
        {
            get;
            set;
        }

        public BlendEquation BlendEquationRgb
        {
            get { return _blendEqRgb.ToBlendEquation(); }
            set { _blendEqRgb = value.ToGLBlendEquation(); }
        }

        public BlendEquation BlendEquationAlpha
        {
            get { return _blendEqAlpha.ToBlendEquation(); }
            set { _blendEqAlpha = value.ToGLBlendEquation(); }
        }

        public BlendOperand BlendSrcRgb
        {
            get { return _blendSrcRgb.ToBlendOperand(); }
            set { _blendSrcRgb = value.ToGLBlendingFactor(); }
        }

        public BlendOperand BlendSrcAlpha
        {
            get { return _blendSrcAlpha.ToBlendOperand(); }
            set { _blendSrcAlpha = value.ToGLBlendingFactor(); }
        }

        public BlendOperand BlendDstRgb
        {
            get { return _blendDstRgb.ToBlendOperand(); }
            set { _blendDstRgb = value.ToGLBlendingFactor(); }
        }

        public BlendOperand BlendDstAlpha
        {
            get { return _blendDstAlpha.ToBlendOperand(); }
            set { _blendDstAlpha = value.ToGLBlendingFactor(); }
        }

        public bool DepthTestEnabled
        {
            get;
            set;
        }

        public CompareFunc DepthFunc
        {
            get { return _depthFunc.ToCompareFunc(); }
            set { _depthFunc = value.ToGLDepthFunction(); }
        }

        public PolygonFace CullFace
        {
            get { return _cullFace.ToPolygonFace(); }
            set { _cullFace = value.ToGLCullFaceMode(); }
        }

        public PolygonWinding PolygonWinding
        {
            get { return _frontFace.ToPolygonWinding(); }
            set { _frontFace = value.ToGLFrontFaceDirection(); }
        }

        public PrimitiveType PrimitiveType
        {
            get { return _primitiveType.ToPrimitiveType(); }
            set { _primitiveType = value.ToGLPrimitiveType(); }
        }

        public bool WriteRed
        {
            get;
            set;
        }

        public bool WriteGreen
        {
            get;
            set;
        }

        public bool WriteBlue
        {
            get;
            set;
        }

        public bool WriteAlpha
        {
            get;
            set;
        }

        public bool WriteDepth
        {
            get;
            set;
        }

        public int BaseVertex
        {
            get;
            set;
        }

        public float LineWidth
        {
            get;
            set;
        }

        public GLDrawCall(GLProgram program)
            : this()
        {
            if (_boundAttributes == null)
                _boundAttributes = new List<int>();

            _constValues = new string[program.ConstantCount];

            Program = program;

            BlendEnabled = false;
            BlendEquationRgb = BlendEquationAlpha = BlendEquation.Add;
            BlendSrcRgb = BlendSrcAlpha = BlendDstRgb = BlendDstAlpha = BlendOperand.One;
            WriteRed = WriteGreen = WriteBlue = WriteAlpha = WriteDepth = DepthTestEnabled = true;
            LineWidth = 1.0f;
            DepthFunc = CompareFunc.LessOrEqual;
            CullFace = PolygonFace.Back;
            PolygonWinding = PolygonWinding.CounterClockwise;
            PrimitiveType = PrimitiveType.Triangles;
            BaseVertex = 0;
        }

        void ConstInternal(int index, string value)
        {
            if (_constValues != null && _constValues[index] != value)
            {
                _constValues[index] = value;
                _compiledProgramDirty = true;
            }
        }

        public void Const(int index, bool value)
        {
            ConstInternal(index, value ? "true" : "false");
        }

        public void Const(int index, int value)
        {
            ConstInternal(index, value.ToString());
        }

        public void Use()
        {
            if (Program == null)
                throw new Uno.InvalidOperationException("Draw statements may not be used from the constructor of the containing class.");

            if (_compiledProgramDirty || _compiledProgram == null)
            {
                _compiledProgram = Program.GetCompiledProgram(_constValues);
                _compiledProgramDirty = false;
            }

            GL.UseProgram(_compiledProgram.GLProgramHandle);
        }

        public void Attrib(int index, int componentCount, GLDataType componentType, bool normalized, VertexBuffer buf, int stride, int offset)
        {
            if (buf == null)
                return;

            var location = _compiledProgram.GetLocation(index);

            if (location < 0)
                return;

            GL.EnableVertexAttribArray(location);
            GL.BindBuffer(GLBufferTarget.ArrayBuffer, buf.GLBufferHandle);
            GL.VertexAttribPointer(location, componentCount, componentType, normalized, stride, offset);
            GL.BindBuffer(GLBufferTarget.ArrayBuffer, GLBufferHandle.Zero);

            _boundAttributes.Add(location);
        }

        public void Attrib(int index, VertexAttributeType type, VertexBuffer buf, int stride, int offset)
        {
            int componentCount;
            GLDataType componentType;
            bool normalized;
            type.ToGLVertexAttributeType(out componentCount, out componentType, out normalized);
            Attrib(index, componentCount, componentType, normalized, buf, stride, offset);
        }

        public void Sampler(int index, GLTextureTarget target, GLTextureHandle handle, bool isMipmap, bool isPow2)
        {
            var location = _compiledProgram.GetLocation(index);

            GL.ActiveTexture((GLTextureUnit)((int)GLTextureUnit.Texture0 + _currentTextureUnit));
            GL.BindTexture(target, handle);
            GL.TexParameter(target, GLTextureParameterName.MagFilter, GLTextureParameterValue.Linear);

            if (isMipmap)
                GL.TexParameter(target, GLTextureParameterName.MinFilter, GLTextureParameterValue.LinearMipmapLinear);
            else
                GL.TexParameter(target, GLTextureParameterName.MinFilter, GLTextureParameterValue.Linear);

            if (target != GLTextureTarget.TextureExternalOES && (isPow2 || Texture2D.HaveNonPow2Support))
            {
                GL.TexParameter(target, GLTextureParameterName.WrapS, GLTextureParameterValue.Repeat);
                GL.TexParameter(target, GLTextureParameterName.WrapT, GLTextureParameterValue.Repeat);
            }
            else
            {
                GL.TexParameter(target, GLTextureParameterName.WrapS, GLTextureParameterValue.ClampToEdge);
                GL.TexParameter(target, GLTextureParameterName.WrapT, GLTextureParameterValue.ClampToEdge);
            }

            GL.Uniform1(location, _currentTextureUnit++);
        }

        public void Sampler(int index, GLTextureTarget target, GLTextureHandle handle, SamplerState state, bool isMipmap, bool isPow2)
        {
            var location = _compiledProgram.GetLocation(index);

            GL.ActiveTexture((GLTextureUnit)((int)GLTextureUnit.Texture0 + _currentTextureUnit));
            GL.BindTexture(target, handle);
            GL.TexParameter(target, GLTextureParameterName.MagFilter, (GLTextureParameterValue)(int)state.MagFilter);

            if (isMipmap)
                GL.TexParameter(target, GLTextureParameterName.MinFilter, (GLTextureParameterValue)(int)state.MinFilter);
            else
                GL.TexParameter(target, GLTextureParameterName.MinFilter, (GLTextureParameterValue)(int)state.MinFilterNoMipmap);

            if (target != GLTextureTarget.TextureExternalOES && (isPow2 || Texture2D.HaveNonPow2Support))
            {
                GL.TexParameter(target, GLTextureParameterName.WrapS, (GLTextureParameterValue)(int)state.AddressU);
                GL.TexParameter(target, GLTextureParameterName.WrapT, (GLTextureParameterValue)(int)state.AddressV);
            }
            else
            {
                GL.TexParameter(target, GLTextureParameterName.WrapS, GLTextureParameterValue.ClampToEdge);
                GL.TexParameter(target, GLTextureParameterName.WrapT, GLTextureParameterValue.ClampToEdge);
            }

            GL.Uniform1(location, _currentTextureUnit++);
        }

        public void DisableSampler(int index, GLTextureTarget target)
        {
            GL.ActiveTexture((GLTextureUnit)((int)GLTextureUnit.Texture0 + _currentTextureUnit));
            GL.BindTexture(target, GLTextureHandle.Zero);
            _currentTextureUnit++;
        }

        public void Sampler(int index, texture2D value)
        {
            if (value != null)
                Sampler(index, GLTextureTarget.Texture2D, value.GLTextureHandle, value.IsMipmap, value.IsPow2);
            else
                DisableSampler(index, GLTextureTarget.Texture2D);
        }

        public void Sampler(int index, texture2D value, SamplerState state)
        {
            if (value != null)
                Sampler(index, GLTextureTarget.Texture2D, value.GLTextureHandle, state, value.IsMipmap, value.IsPow2);
            else
                DisableSampler(index, GLTextureTarget.Texture2D);
        }

        public void Sampler(int index, textureCube value)
        {
            if (value != null)
                Sampler(index, GLTextureTarget.TextureCubeMap, value.GLTextureHandle, value.IsMipmap, value.IsPow2);
            else
                DisableSampler(index, GLTextureTarget.TextureCubeMap);
        }

        public void Sampler(int index, textureCube value, SamplerState state)
        {
            if (value != null)
                Sampler(index, GLTextureTarget.TextureCubeMap, value.GLTextureHandle, state, value.IsMipmap, value.IsPow2);
            else
                DisableSampler(index, GLTextureTarget.TextureCubeMap);
        }

        public void Sampler(int index, VideoTexture value)
        {
            if (value != null)
                Sampler(index, VideoTextureTarget, value.GLTextureHandle, value.IsMipmap, value.IsPow2);
            else
                DisableSampler(index, VideoTextureTarget);
        }

        public void Sampler(int index, VideoTexture value, SamplerState state)
        {
            if (value != null)
                Sampler(index, VideoTextureTarget, value.GLTextureHandle, state, value.IsMipmap, value.IsPow2);
            else
                DisableSampler(index, VideoTextureTarget);
        }

        public void Uniform(int index, int value)
        {
            GL.Uniform1(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, int2 value)
        {
            GL.Uniform2(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, int3 value)
        {
            GL.Uniform3(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, int4 value)
        {
            GL.Uniform4(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, int[] value)
        {
            GL.Uniform1(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, int2[] value)
        {
            GL.Uniform2(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, int3[] value)
        {
            GL.Uniform3(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, int4[] value)
        {
            GL.Uniform4(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float value)
        {
            GL.Uniform1(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float2 value)
        {
            GL.Uniform2(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float3 value)
        {
            GL.Uniform3(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float4 value)
        {
            GL.Uniform4(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float2x2 value)
        {
            GL.UniformMatrix2(_compiledProgram.GetLocation(index), false, value);
        }

        public void Uniform(int index, float3x3 value)
        {
            GL.UniformMatrix3(_compiledProgram.GetLocation(index), false, value);
        }

        public void Uniform(int index, float4x4 value)
        {
            GL.UniformMatrix4(_compiledProgram.GetLocation(index), false, value);
        }

        public void Uniform(int index, float[] value)
        {
            GL.Uniform1(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float2[] value)
        {
            GL.Uniform2(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float3[] value)
        {
            GL.Uniform3(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float4[] value)
        {
            GL.Uniform4(_compiledProgram.GetLocation(index), value);
        }

        public void Uniform(int index, float2x2[] value)
        {
            GL.UniformMatrix2(_compiledProgram.GetLocation(index), false, value);
        }

        public void Uniform(int index, float3x3[] value)
        {
            GL.UniformMatrix3(_compiledProgram.GetLocation(index), false, value);
        }

        public void Uniform(int index, float4x4[] value)
        {
            GL.UniformMatrix4(_compiledProgram.GetLocation(index), false, value);
        }

        void Begin()
        {
            if (BlendEnabled)
            {
                GL.Enable(GLEnableCap.Blend);
                GL.BlendFuncSeparate(_blendSrcRgb, _blendDstRgb, _blendSrcAlpha, _blendDstAlpha);
            }
            else
            {
                GL.Disable(GLEnableCap.Blend);
            }

            if (LineWidth != 1.0f)
            {
                GL.LineWidth(LineWidth);
            }

            if (DepthTestEnabled)
            {
                GL.Enable(GLEnableCap.DepthTest);
                GL.DepthFunc(_depthFunc);
            }
            else
            {
                GL.Disable(GLEnableCap.DepthTest);
            }

            if (_cullFace != GLCullFaceMode.None)
            {
                GL.Enable(GLEnableCap.CullFace);
                GL.CullFace(_cullFace);
                GL.FrontFace(_frontFace);
            }
            else
            {
                GL.Disable(GLEnableCap.CullFace);
            }

            GL.DepthMask(WriteDepth);
            GL.ColorMask(WriteRed, WriteGreen, WriteBlue, WriteAlpha);
        }

        void End()
        {
            for (int i = 0; i < _boundAttributes.Count; i++)
            {
                GL.DisableVertexAttribArray(_boundAttributes[i]);
            }

            for (int i = 0; i < _currentTextureUnit; i++)
            {
                GL.ActiveTexture((GLTextureUnit)((int)GLTextureUnit.Texture0 + i));
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle.Zero);
            }

            if (LineWidth != 1.0f)
            {
                GL.LineWidth(1.0f);
            }

            GL.DepthMask(true);
            GL.ColorMask(true, true, true, true);

            _boundAttributes.Clear();
            _currentTextureUnit = 0;
        }

        public void DrawArrays(int count)
        {
            Begin();
            GL.DrawArrays(_primitiveType, BaseVertex, count);
            End();
        }

        public void DrawElements(int count, GLIndexType type, IndexBuffer buf)
        {
            Begin();
            GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, buf.GLBufferHandle);
            GL.DrawElements(_primitiveType, count, type, BaseVertex);
            GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, GLBufferHandle.Zero);
            End();
        }

        public void Draw(int count, IndexType type = IndexType.Undefined, IndexBuffer buf = null)
        {
            if (type == IndexType.Undefined)
                DrawArrays(count);
            else
                DrawElements(count, type.ToGLIndexType(), buf);
        }
    }
}
