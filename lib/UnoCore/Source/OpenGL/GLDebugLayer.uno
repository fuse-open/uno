using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Diagnostics;
using System.Diagnostics;
using System.Reflection;

namespace OpenGL
{
    extern(DOTNET)
    class GLDebugLayer : IGL
    {
        readonly IGL _gl;

        public GLDebugLayer(IGL gl)
        {
            _gl = gl;
        }

        public GLError GetError()
        {
            try { BeginFunc(); return _gl.GetError(); } finally { EndFunc(); }
        }

        public void Finish()
        {
            try { BeginFunc(); _gl.Finish(); } finally { EndFunc(); }
        }

        public void Flush()
        {
            try { BeginFunc(); _gl.Flush(); } finally { EndFunc(); }
        }

        public GLFramebufferStatus CheckFramebufferStatus(GLFramebufferTarget target)
        {
            try { BeginFunc(); return _gl.CheckFramebufferStatus(target); } finally { EndFunc(); }
        }

        public int GetInteger(GLIntegerName name)
        {
            try { BeginFunc(); return _gl.GetInteger(name); } finally { EndFunc(); }
        }

        public int4 GetInteger(GLInteger4Name name)
        {
            try { BeginFunc(); return _gl.GetInteger(name); } finally { EndFunc(); }
        }

        public string GetString(GLStringName name)
        {
            try { BeginFunc(); return _gl.GetString(name); } finally { EndFunc(); }
        }

        public void Clear(GLClearBufferMask mask)
        {
            try { BeginFunc(); _gl.Clear(mask); } finally { EndFunc(); }
        }
        public void ClearColor(float red, float green, float blue, float alpha)
        {
            try { BeginFunc(); _gl.ClearColor(red, green, blue, alpha); } finally { EndFunc(); }
        }
        public void ClearDepth(float depth)
        {
            try { BeginFunc(); _gl.ClearDepth(depth); } finally { EndFunc(); }
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            try { BeginFunc(); _gl.ColorMask(red, green, blue, alpha); } finally { EndFunc(); }
        }
        public void DepthMask(bool flag)
        {
            try { BeginFunc(); _gl.DepthMask(flag); } finally { EndFunc(); }
        }

        public GLTextureHandle CreateTexture()
        {
            try { BeginFunc(); return _gl.CreateTexture(); } finally { EndFunc(); }
        }
        public void DeleteTexture(GLTextureHandle texture)
        {
            try { BeginFunc(); _gl.DeleteTexture(texture); } finally { EndFunc(); }
        }
        public void ActiveTexture(GLTextureUnit texture)
        {
            try { BeginFunc(); _gl.ActiveTexture(texture); } finally { EndFunc(); }
        }
        public void BindTexture(GLTextureTarget target, GLTextureHandle texture)
        {
            try { BeginFunc(); _gl.BindTexture(target, texture); } finally { EndFunc(); }
        }
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureParameterValue param)
        {
            try { BeginFunc(); _gl.TexParameter(target, pname, param); } finally { EndFunc(); }
        }
        public void TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, IntPtr data)
        {
            try { BeginFunc(); _gl.TexImage2D(target, level, internalFormat, width, height, border, format, type, data); } finally { EndFunc(); }
        }
        public void TexSubImage2D(GLTextureTarget target, int level, int xoffset, int yoffset, int width, int height, GLPixelFormat format, GLPixelType type, IntPtr data)
        {
            try { BeginFunc(); _gl.TexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, data); } finally { EndFunc(); }
        }
        public void GenerateMipmap(GLTextureTarget target)
        {
            try { BeginFunc(); _gl.GenerateMipmap(target); } finally { EndFunc(); }
        }
        public void PixelStore(GLPixelStoreParameter pname, int param)
        {
            try { BeginFunc(); _gl.PixelStore(pname, param); } finally { EndFunc(); }
        }

        public void ReadPixels(int x, int y, int width, int height, GLPixelFormat format, GLPixelType type, byte[] buffer)
        {
            try { BeginFunc(); _gl.ReadPixels(x, y, width, height, format, type, buffer); } finally { EndFunc(); }
        }

        public GLRenderbufferHandle CreateRenderbuffer()
        {
            try { BeginFunc(); return _gl.CreateRenderbuffer(); } finally { EndFunc(); }
        }
        public void DeleteRenderbuffer(GLRenderbufferHandle renderbuffer)
        {
            try { BeginFunc(); _gl.DeleteRenderbuffer(renderbuffer); } finally { EndFunc(); }
        }
        public void BindRenderbuffer(GLRenderbufferTarget target, GLRenderbufferHandle renderbuffer)
        {
            try { BeginFunc(); _gl.BindRenderbuffer(target, renderbuffer); } finally { EndFunc(); }
        }
        public void RenderbufferStorage(GLRenderbufferTarget target, GLRenderbufferStorage internalFormat, int width, int height)
        {
            try { BeginFunc(); _gl.RenderbufferStorage(target, internalFormat, width, height); } finally { EndFunc(); }
        }
        public GLRenderbufferHandle GetRenderbufferBinding()
        {
            try { BeginFunc(); return _gl.GetRenderbufferBinding(); } finally { EndFunc(); }
        }

        public GLFramebufferHandle CreateFramebuffer()
        {
            try { BeginFunc(); return _gl.CreateFramebuffer(); } finally { EndFunc(); }
        }
        public void DeleteFramebuffer(GLFramebufferHandle fb)
        {
            try { BeginFunc(); _gl.DeleteFramebuffer(fb); } finally { EndFunc(); }
        }
        public void BindFramebuffer(GLFramebufferTarget target, GLFramebufferHandle fb)
        {
            try { BeginFunc(); _gl.BindFramebuffer(target, fb); } finally { EndFunc(); }
        }
        public void FramebufferTexture2D(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLTextureTarget textarget, GLTextureHandle texture, int level)
        {
            try { BeginFunc(); _gl.FramebufferTexture2D(target, attachment, textarget, texture, level); } finally { EndFunc(); }
        }
        public void FramebufferRenderbuffer(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLRenderbufferTarget renderbuffertarget, GLRenderbufferHandle renderbuffer)
        {
            try { BeginFunc(); _gl.FramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer); } finally { EndFunc(); }
        }
        public GLFramebufferHandle GetFramebufferBinding()
        {
            try { BeginFunc(); return _gl.GetFramebufferBinding(); } finally { EndFunc(); }
        }

        public void UseProgram(GLProgramHandle program)
        {
            try { BeginFunc(); _gl.UseProgram(program); } finally { EndFunc(); }
        }
        public void BindAttribLocation(GLProgramHandle program, int index, string name)
        {
            try { BeginFunc(); _gl.BindAttribLocation(program, index, name); } finally { EndFunc(); }
        }
        public int GetAttribLocation(GLProgramHandle program, string name)
        {
            try { BeginFunc(); return _gl.GetAttribLocation(program, name); } finally { EndFunc(); }
        }
        public int GetUniformLocation(GLProgramHandle program, string name)
        {
            try { BeginFunc(); return _gl.GetUniformLocation(program, name); } finally { EndFunc(); }
        }

        public void Uniform1(int location, int value)
        {
            try { BeginFunc(); _gl.Uniform1(location, value); } finally { EndFunc(); }
        }
        public void Uniform2(int location, int2 value)
        {
            try { BeginFunc(); _gl.Uniform2(location, value); } finally { EndFunc(); }
        }
        public void Uniform3(int location, int3 value)
        {
            try { BeginFunc(); _gl.Uniform3(location, value); } finally { EndFunc(); }
        }
        public void Uniform4(int location, int4 value)
        {
            try { BeginFunc(); _gl.Uniform4(location, value); } finally { EndFunc(); }
        }
        public void Uniform1(int location, float value)
        {
            try { BeginFunc(); _gl.Uniform1(location, value); } finally { EndFunc(); }
        }
        public void Uniform2(int location, float2 value)
        {
            try { BeginFunc(); _gl.Uniform2(location, value); } finally { EndFunc(); }
        }
        public void Uniform3(int location, float3 value)
        {
            try { BeginFunc(); _gl.Uniform3(location, value); } finally { EndFunc(); }
        }
        public void Uniform4(int location, float4 value)
        {
            try { BeginFunc(); _gl.Uniform4(location, value); } finally { EndFunc(); }
        }
        public void UniformMatrix2(int location, bool transpose, float2x2 value)
        {
            try { BeginFunc(); _gl.UniformMatrix2(location, transpose, value); } finally { EndFunc(); }
        }
        public void UniformMatrix3(int location, bool transpose, float3x3 value)
        {
            try { BeginFunc(); _gl.UniformMatrix3(location, transpose, value); } finally { EndFunc(); }
        }
        public void UniformMatrix4(int location, bool transpose, float4x4 value)
        {
            try { BeginFunc(); _gl.UniformMatrix4(location, transpose, value); } finally { EndFunc(); }
        }

        public void Uniform1(int location, int[] value)
        {
            try { BeginFunc(); _gl.Uniform1(location, value); } finally { EndFunc(); }
        }
        public void Uniform2(int location, int2[] value)
        {
            try { BeginFunc(); _gl.Uniform2(location, value); } finally { EndFunc(); }
        }
        public void Uniform3(int location, int3[] value)
        {
            try { BeginFunc(); _gl.Uniform3(location, value); } finally { EndFunc(); }
        }
        public void Uniform4(int location, int4[] value)
        {
            try { BeginFunc(); _gl.Uniform4(location, value); } finally { EndFunc(); }
        }
        public void Uniform1(int location, float[] value)
        {
            try { BeginFunc(); _gl.Uniform1(location, value); } finally { EndFunc(); }
        }
        public void Uniform2(int location, float2[] value)
        {
            try { BeginFunc(); _gl.Uniform2(location, value); } finally { EndFunc(); }
        }
        public void Uniform3(int location, float3[] value)
        {
            try { BeginFunc(); _gl.Uniform3(location, value); } finally { EndFunc(); }
        }
        public void Uniform4(int location, float4[] value)
        {
            try { BeginFunc(); _gl.Uniform4(location, value); } finally { EndFunc(); }
        }
        public void UniformMatrix2(int location, bool transpose, float2x2[] value)
        {
            try { BeginFunc(); _gl.UniformMatrix2(location, transpose, value); } finally { EndFunc(); }
        }
        public void UniformMatrix3(int location, bool transpose, float3x3[] value)
        {
            try { BeginFunc(); _gl.UniformMatrix3(location, transpose, value); } finally { EndFunc(); }
        }
        public void UniformMatrix4(int location, bool transpose, float4x4[] value)
        {
            try { BeginFunc(); _gl.UniformMatrix4(location, transpose, value); } finally { EndFunc(); }
        }

        public void EnableVertexAttribArray(int index)
        {
            try { BeginFunc(); _gl.EnableVertexAttribArray(index); } finally { EndFunc(); }
        }
        public void DisableVertexAttribArray(int index)
        {
            try { BeginFunc(); _gl.DisableVertexAttribArray(index); } finally { EndFunc(); }
        }
        public void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
        {
            try { BeginFunc(); _gl.VertexAttribPointer(index, size, type, normalized, stride, offset); } finally { EndFunc(); }
        }

        public void DrawArrays(GLPrimitiveType mode, int first, int count)
        {
            try { BeginFunc(); _gl.DrawArrays(mode, first, count); } finally { EndFunc(); }
        }
        public void DrawElements(GLPrimitiveType mode, int count, GLIndexType type, int offset)
        {
            try { BeginFunc(); _gl.DrawElements(mode, count, type, offset); } finally { EndFunc(); }
        }

        public GLBufferHandle CreateBuffer()
        {
            try { BeginFunc(); return _gl.CreateBuffer(); } finally { EndFunc(); }
        }
        public void DeleteBuffer(GLBufferHandle buffer)
        {
            try { BeginFunc(); _gl.DeleteBuffer(buffer); } finally { EndFunc(); }
        }
        public void BindBuffer(GLBufferTarget target, GLBufferHandle buffer)
        {
            try { BeginFunc(); _gl.BindBuffer(target, buffer); } finally { EndFunc(); }
        }
        public void BufferData(GLBufferTarget target, int sizeInBytes, IntPtr data, GLBufferUsage usage)
        {
            try { BeginFunc(); _gl.BufferData(target, sizeInBytes, data, usage); } finally { EndFunc(); }
        }
        public void BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, IntPtr data)
        {
            try { BeginFunc(); _gl.BufferSubData(target, offset, sizeInBytes, data); } finally { EndFunc(); }
        }

        public void Enable(GLEnableCap cap)
        {
            try { BeginFunc(); _gl.Enable(cap); } finally { EndFunc(); }
        }
        public void Disable(GLEnableCap cap)
        {
            try { BeginFunc(); _gl.Disable(cap); } finally { EndFunc(); }
        }

        public void BlendFunc(GLBlendingFactor src, GLBlendingFactor dst)
        {
            try { BeginFunc(); _gl.BlendFunc(src, dst); } finally { EndFunc(); }
        }
        public void BlendFuncSeparate(GLBlendingFactor srcRGB, GLBlendingFactor dstRGB, GLBlendingFactor srcAlpha, GLBlendingFactor dstAlpha)
        {
            try { BeginFunc(); _gl.BlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha); } finally { EndFunc(); }
        }

        public void BlendEquation(GLBlendEquation mode)
        {
            try { BeginFunc(); _gl.BlendEquation(mode); } finally { EndFunc(); }
        }
        public void BlendEquationSeparate(GLBlendEquation modeRgb, GLBlendEquation modeAlpha)
        {
            try { BeginFunc(); _gl.BlendEquationSeparate(modeRgb, modeAlpha); } finally { EndFunc(); }
        }

        public void CullFace(GLCullFaceMode mode)
        {
            try { BeginFunc(); _gl.CullFace(mode); } finally { EndFunc(); }
        }
        public void FrontFace(GLFrontFaceDirection mode)
        {
            try { BeginFunc(); _gl.FrontFace(mode); } finally { EndFunc(); }
        }
        public void DepthFunc(GLDepthFunction func)
        {
            try { BeginFunc(); _gl.DepthFunc(func); } finally { EndFunc(); }
        }

        public void Scissor(int x, int y, int width, int height)
        {
            try { BeginFunc(); _gl.Scissor(x, y, width, height); } finally { EndFunc(); }
        }
        public void Viewport(int x, int y, int width, int height)
        {
            try { BeginFunc(); _gl.Viewport(x, y, width, height); } finally { EndFunc(); }
        }

        public void LineWidth(float width)
        {
            try { BeginFunc(); _gl.LineWidth(width); } finally { EndFunc(); }
        }
        public void PolygonOffset(float factor, float units)
        {
            try { BeginFunc(); _gl.PolygonOffset(factor, units); } finally { EndFunc(); }
        }
        public void DepthRange(float zNear, float zFar)
        {
            try { BeginFunc(); _gl.DepthRange(zNear, zFar); } finally { EndFunc(); }
        }

        public GLShaderHandle CreateShader(GLShaderType type)
        {
            try { BeginFunc(); return _gl.CreateShader(type); } finally { EndFunc(); }
        }
        public void DeleteShader(GLShaderHandle shader)
        {
            try { BeginFunc(); _gl.DeleteShader(shader); } finally { EndFunc(); }
        }
        public void ShaderSource(GLShaderHandle shader, string source)
        {
            try { BeginFunc(); _gl.ShaderSource(shader, source); } finally { EndFunc(); }
        }
        public void CompileShader(GLShaderHandle shader)
        {
            try { BeginFunc(); _gl.CompileShader(shader); } finally { EndFunc(); }
        }
        public int GetShaderParameter(GLShaderHandle shader, GLShaderParameter pname)
        {
            try { BeginFunc(); return _gl.GetShaderParameter(shader, pname); } finally { EndFunc(); }
        }
        public string GetShaderInfoLog(GLShaderHandle shader)
        {
            try { BeginFunc(); return _gl.GetShaderInfoLog(shader); } finally { EndFunc(); }
        }

        public GLProgramHandle CreateProgram()
        {
            try { BeginFunc(); return _gl.CreateProgram(); } finally { EndFunc(); }
        }
        public void DeleteProgram(GLProgramHandle program)
        {
            try { BeginFunc(); _gl.DeleteProgram(program); } finally { EndFunc(); }
        }
        public void AttachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            try { BeginFunc(); _gl.AttachShader(program, shader); } finally { EndFunc(); }
        }
        public void DetachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            try { BeginFunc(); _gl.DetachShader(program, shader); } finally { EndFunc(); }
        }
        public void LinkProgram(GLProgramHandle program)
        {
            try { BeginFunc(); _gl.LinkProgram(program); } finally { EndFunc(); }
        }
        public int GetProgramParameter(GLProgramHandle program, GLProgramParameter pname)
        {
            try { BeginFunc(); return _gl.GetProgramParameter(program, pname); } finally { EndFunc(); }
        }
        public string GetProgramInfoLog(GLProgramHandle program)
        {
            try { BeginFunc(); return _gl.GetProgramInfoLog(program); } finally { EndFunc(); }
        }
        public bool HasGetShaderPrecisionFormat
        {
            get { return _gl.HasGetShaderPrecisionFormat; }
        }
        public GLShaderPrecisionFormat GetShaderPrecisionFormat(GLShaderType shader, GLShaderPrecision precision)
        {
            try { BeginFunc(); return _gl.GetShaderPrecisionFormat(shader, precision); } finally { EndFunc(); }
        }

        void CheckError()
        {
            var err = _gl.GetError();
            if (err != 0)
            {
                Debug.Log("GL error (" + err + ")", DebugMessageType.Error);
                var frames = new StackTrace().GetFrames();

                for (int i = 3; i < frames.Length; i++)
                {
                    var f = frames[i];
                    if (f.GetMethod().Name == "Main")
                        break;

                    Debug.Log("in " + f.GetMethod().DeclaringType + "." + f.GetMethod().Name + "()");
                }
            }
        }

        void BeginFunc()
        {
            CheckError();
        }

        void EndFunc()
        {
            CheckError();
        }
    }
}

namespace System.Diagnostics
{
    [DotNetType]
    extern(DOTNET) class StackTrace
    {
        public extern StackTrace();
        public extern virtual StackFrame[] GetFrames();
    }

    [DotNetType]
    extern(DOTNET) class StackFrame
    {
        public extern virtual MethodBase GetMethod();
    }
}

namespace System.Reflection
{
    [DotNetType]
    extern(DOTNET) abstract class MethodBase
    {
        public abstract Type DeclaringType { get; }
        public abstract string Name { get; }
    }
}
