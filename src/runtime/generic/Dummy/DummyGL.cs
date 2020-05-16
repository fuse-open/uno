using System;
using OpenGL;

namespace Uno.AppLoader.Dummy
{
    public class DummyGL : IGL
    {
        public GLError GetError()
        {
            return GLError.NoError;
        }

        public void Finish()
        {
        }

        public void Flush()
        {
        }

        public int GetInteger(GLIntegerName pname)
        {
            return 0;
        }

        public Int4 GetInteger(GLInteger4Name pname)
        {
            return new Int4(0, 0, 0, 0);
        }

        public GLFramebufferStatus CheckFramebufferStatus(GLFramebufferTarget target)
        {
            return GLFramebufferStatus.FramebufferUnsupported;
        }

        public void Clear(GLClearBufferMask mask)
        {
        }

        public void ClearColor(float red, float green, float blue, float alpha)
        {
        }

        public void ClearDepth(float depth)
        {
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
        }

        public void DepthMask(bool flag)
        {
        }

        public GLTextureHandle CreateTexture()
        {
            return new GLTextureHandle(0);
        }

        public void DeleteTexture(GLTextureHandle texture)
        {
        }

        public void ActiveTexture(GLTextureUnit texture)
        {
        }

        public void BindTexture(GLTextureTarget target, GLTextureHandle texture)
        {
        }

        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureParameterValue param)
        {
        }

        public void TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, IntPtr data)
        {
        }

        public void TexSubImage2D(GLTextureTarget target,
                                  int level,
                                  int xoffset,
                                  int yoffset,
                                  int width,
                                  int height,
                                  GLPixelFormat format,
                                  GLPixelType type,
                                  IntPtr data)
        {
        }

        public void GenerateMipmap(GLTextureTarget target)
        {
        }

        public void PixelStore(GLPixelStoreParameter pname, int param)
        {
        }

        public GLRenderbufferHandle CreateRenderbuffer()
        {
            return new GLRenderbufferHandle(0);
        }

        public void DeleteRenderbuffer(GLRenderbufferHandle renderbuffer)
        {
        }

        public void BindRenderbuffer(GLRenderbufferTarget target, GLRenderbufferHandle renderbuffer)
        {
        }

        public void RenderbufferStorage(GLRenderbufferTarget target, GLRenderbufferStorage internalFormat, int width, int height)
        {
        }

        public GLFramebufferHandle CreateFramebuffer()
        {
            return new GLFramebufferHandle(0);
        }

        public void DeleteFramebuffer(GLFramebufferHandle fb)
        {
        }

        public void BindFramebuffer(GLFramebufferTarget target, GLFramebufferHandle fb)
        {
        }

        public void FramebufferTexture2D(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLTextureTarget textarget, GLTextureHandle texture, int level)
        {
        }

        public void FramebufferRenderbuffer(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLRenderbufferTarget renderbuffertarget, GLRenderbufferHandle renderbuffer)
        {
        }

        public void UseProgram(GLProgramHandle program)
        {
        }

        public void BindAttribLocation(GLProgramHandle program, int index, string name)
        {
        }

        public int GetAttribLocation(GLProgramHandle program, string name)
        {
            return 0;
        }

        public int GetUniformLocation(GLProgramHandle program, string name)
        {
            return 0;
        }

        public void Uniform1(int location, int value)
        {
        }

        public void Uniform2(int location, Int2 value)
        {
        }

        public void Uniform3(int location, Int3 value)
        {
        }

        public void Uniform4(int location, Int4 value)
        {
        }

        public void Uniform1(int location, float value)
        {
        }

        public void Uniform2(int location, Float2 value)
        {
        }

        public void Uniform3(int location, Float3 value)
        {
        }

        public void Uniform4(int location, Float4 value)
        {
        }

        public void UniformMatrix2(int location, bool transpose, Float2x2 value)
        {
        }

        public void UniformMatrix3(int location, bool transpose, Float3x3 value)
        {
        }

        public void UniformMatrix4(int location, bool transpose, Float4x4 value)
        {
        }

        public void Uniform1(int location, int[] value)
        {
        }

        public void Uniform2(int location, Int2[] value)
        {
        }

        public void Uniform3(int location, Int3[] value)
        {
        }

        public void Uniform4(int location, Int4[] value)
        {
        }

        public void Uniform1(int location, float[] value)
        {
        }

        public void Uniform2(int location, Float2[] value)
        {
        }

        public void Uniform3(int location, Float3[] value)
        {
        }

        public void Uniform4(int location, Float4[] value)
        {
        }

        public void UniformMatrix2(int location, bool transpose, Float2x2[] value)
        {
        }

        public void UniformMatrix3(int location, bool transpose, Float3x3[] value)
        {
        }

        public void UniformMatrix4(int location, bool transpose, Float4x4[] value)
        {
        }

        public void EnableVertexAttribArray(int index)
        {
        }

        public void DisableVertexAttribArray(int index)
        {
        }

        public void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
        {
        }

        public void DrawArrays(GLPrimitiveType mode, int first, int count)
        {
        }

        public void DrawElements(GLPrimitiveType mode, int count, GLIndexType type, int offset)
        {
        }

        public GLBufferHandle CreateBuffer()
        {
            return new GLBufferHandle(0);
        }

        public void DeleteBuffer(GLBufferHandle buffer)
        {
        }

        public void BindBuffer(GLBufferTarget target, GLBufferHandle buffer)
        {
        }

        public void BufferData(GLBufferTarget target, int sizeInBytes, IntPtr data, GLBufferUsage usage)
        {
        }

        public void BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, IntPtr data)
        {
        }

        public void Enable(GLEnableCap cap)
        {
        }

        public void Disable(GLEnableCap cap)
        {
        }

        public bool IsEnabled(GLEnableCap cap)
        {
            return false;
        }

        public void BlendFunc(GLBlendingFactor src, GLBlendingFactor dst)
        {
        }

        public void BlendFuncSeparate(GLBlendingFactor srcRGB, GLBlendingFactor dstRGB, GLBlendingFactor srcAlpha, GLBlendingFactor dstAlpha)
        {
        }

        public void BlendEquation(GLBlendEquation mode)
        {
        }

        public void BlendEquationSeparate(GLBlendEquation modeRgb, GLBlendEquation modeAlpha)
        {
        }

        public void CullFace(GLCullFaceMode mode)
        {
        }

        public void FrontFace(GLFrontFaceDirection mode)
        {
        }

        public void DepthFunc(GLDepthFunction func)
        {
        }

        public void Scissor(int x, int y, int width, int height)
        {
        }

        public void Viewport(int x, int y, int width, int height)
        {
        }

        public void LineWidth(float width)
        {
        }

        public void PolygonOffset(float factor, float units)
        {
        }

        public void DepthRange(float zNear, float zFar)
        {
        }

        public GLShaderHandle CreateShader(GLShaderType type)
        {
            return new GLShaderHandle(0);
        }

        public void DeleteShader(GLShaderHandle shader)
        {
        }

        public void ShaderSource(GLShaderHandle shader, string source)
        {
        }

        public void ReadPixels(int x, int y, int width, int height, GLPixelFormat format, GLPixelType type, byte[] buffer)
        {
        }

        public void CompileShader(GLShaderHandle shader)
        {
        }

        public int GetShaderParameter(GLShaderHandle shader, GLShaderParameter pname)
        {
            return 0;
        }

        public string GetShaderInfoLog(GLShaderHandle shader)
        {
            return "";
        }

        public GLProgramHandle CreateProgram()
        {
            return new GLProgramHandle(0);
        }

        public void DeleteProgram(GLProgramHandle program)
        {
        }

        public void AttachShader(GLProgramHandle program, GLShaderHandle shader)
        {
        }

        public void DetachShader(GLProgramHandle program, GLShaderHandle shader)
        {
        }

        public void LinkProgram(GLProgramHandle program)
        {
        }

        public int GetProgramParameter(GLProgramHandle program, GLProgramParameter pname)
        {
            return 0;
        }

        public string GetProgramInfoLog(GLProgramHandle program)
        {
            return "";
        }

        public bool HasGetShaderPrecisionFormat => false;

        public GLShaderPrecisionFormat GetShaderPrecisionFormat(GLShaderType shaderType, GLShaderPrecision precision)
        {
            return new GLShaderPrecisionFormat();
        }

        public string GetString(GLStringName name)
        {
            return "2.0";
        }

        public GLRenderbufferHandle GetRenderbufferBinding()
        {
            return new GLRenderbufferHandle(0);
        }

        public GLFramebufferHandle GetFramebufferBinding()
        {
            return new GLFramebufferHandle(0);
        }
    }
}
