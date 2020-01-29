using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace OpenGL
{
    extern(DOTNET)
    public interface IGL
    {
        GLError GetError();
        void Finish();
        void Flush();

        GLFramebufferStatus CheckFramebufferStatus(GLFramebufferTarget target);

        int GetInteger(GLIntegerName name);
        int4 GetInteger(GLInteger4Name name);
        string GetString(GLStringName name);

        void Clear(GLClearBufferMask mask);
        void ClearColor(float red, float green, float blue, float alpha);
        void ClearDepth(float depth);

        void ColorMask(bool red, bool green, bool blue, bool alpha);
        void DepthMask(bool flag);

        GLTextureHandle CreateTexture();
        void DeleteTexture(GLTextureHandle texture);
        void ActiveTexture(GLTextureUnit texture);
        void BindTexture(GLTextureTarget target, GLTextureHandle texture);
        void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureParameterValue param);
        void TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, IntPtr data);
        void TexSubImage2D(GLTextureTarget target, int level, int xoffset, int yoffset, int width, int height, GLPixelFormat format, GLPixelType type, IntPtr data);
        void GenerateMipmap(GLTextureTarget target);
        void PixelStore(GLPixelStoreParameter pname, int param);

        void ReadPixels(int x, int y, int width, int height, GLPixelFormat format, GLPixelType type, byte[] buffer);

        GLRenderbufferHandle CreateRenderbuffer();
        void DeleteRenderbuffer(GLRenderbufferHandle renderbuffer);
        void BindRenderbuffer(GLRenderbufferTarget target, GLRenderbufferHandle renderbuffer);
        void RenderbufferStorage(GLRenderbufferTarget target, GLRenderbufferStorage internalFormat, int width, int height);
        GLRenderbufferHandle GetRenderbufferBinding();

        GLFramebufferHandle CreateFramebuffer();
        void DeleteFramebuffer(GLFramebufferHandle fb);
        void BindFramebuffer(GLFramebufferTarget target, GLFramebufferHandle fb);
        void FramebufferTexture2D(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLTextureTarget textarget, GLTextureHandle texture, int level);
        void FramebufferRenderbuffer(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLRenderbufferTarget renderbuffertarget, GLRenderbufferHandle renderbuffer);
        GLFramebufferHandle GetFramebufferBinding();

        void UseProgram(GLProgramHandle program);
        void BindAttribLocation(GLProgramHandle program, int index, string name);
        int GetAttribLocation(GLProgramHandle program, string name);
        int GetUniformLocation(GLProgramHandle program, string name);

        void Uniform1(int location, int value);
        void Uniform2(int location, int2 value);
        void Uniform3(int location, int3 value);
        void Uniform4(int location, int4 value);
        void Uniform1(int location, float value);
        void Uniform2(int location, float2 value);
        void Uniform3(int location, float3 value);
        void Uniform4(int location, float4 value);
        void UniformMatrix2(int location, bool transpose, float2x2 value);
        void UniformMatrix3(int location, bool transpose, float3x3 value);
        void UniformMatrix4(int location, bool transpose, float4x4 value);

        void Uniform1(int location, int[] value);
        void Uniform2(int location, int2[] value);
        void Uniform3(int location, int3[] value);
        void Uniform4(int location, int4[] value);
        void Uniform1(int location, float[] value);
        void Uniform2(int location, float2[] value);
        void Uniform3(int location, float3[] value);
        void Uniform4(int location, float4[] value);
        void UniformMatrix2(int location, bool transpose, float2x2[] value);
        void UniformMatrix3(int location, bool transpose, float3x3[] value);
        void UniformMatrix4(int location, bool transpose, float4x4[] value);

        void EnableVertexAttribArray(int index);
        void DisableVertexAttribArray(int index);
        void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset);

        void DrawArrays(GLPrimitiveType mode, int first, int count);
        void DrawElements(GLPrimitiveType mode, int count, GLIndexType type, int offset);

        GLBufferHandle CreateBuffer();
        void DeleteBuffer(GLBufferHandle buffer);
        void BindBuffer(GLBufferTarget target, GLBufferHandle buffer);
        void BufferData(GLBufferTarget target, int sizeInBytes, IntPtr data, GLBufferUsage usage);
        void BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, IntPtr data);

        void Enable(GLEnableCap cap);
        void Disable(GLEnableCap cap);

        void BlendFunc(GLBlendingFactor src, GLBlendingFactor dst);
        void BlendFuncSeparate(GLBlendingFactor srcRGB, GLBlendingFactor dstRGB, GLBlendingFactor srcAlpha, GLBlendingFactor dstAlpha);

        void BlendEquation(GLBlendEquation mode);
        void BlendEquationSeparate(GLBlendEquation modeRgb, GLBlendEquation modeAlpha);

        void CullFace(GLCullFaceMode mode);
        void FrontFace(GLFrontFaceDirection mode);
        void DepthFunc(GLDepthFunction func);

        void Scissor(int x, int y, int width, int height);
        void Viewport(int x, int y, int width, int height);

        void LineWidth(float width);
        void PolygonOffset(float factor, float units);
        void DepthRange(float zNear, float zFar);

        GLShaderHandle CreateShader(GLShaderType type);
        void DeleteShader(GLShaderHandle shader);
        void ShaderSource(GLShaderHandle shader, string source);
        void CompileShader(GLShaderHandle shader);
        int GetShaderParameter(GLShaderHandle shader, GLShaderParameter pname);
        string GetShaderInfoLog(GLShaderHandle shader);
        bool HasGetShaderPrecisionFormat { get; }
        GLShaderPrecisionFormat GetShaderPrecisionFormat(GLShaderType shader, GLShaderPrecision precision);

        GLProgramHandle CreateProgram();
        void DeleteProgram(GLProgramHandle program);
        void AttachShader(GLProgramHandle program, GLShaderHandle shader);
        void DetachShader(GLProgramHandle program, GLShaderHandle shader);
        void LinkProgram(GLProgramHandle program);
        int GetProgramParameter(GLProgramHandle program, GLProgramParameter pname);
        string GetProgramInfoLog(GLProgramHandle program);
    }
}
