// This file was generated based on Library/Core/UnoCore/Source/OpenGL/GL.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace OpenGL
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class GL
    {
        public static IGL _gl;

        public static int GetInteger(GLIntegerName name)
        {
            return GL._gl.GetInteger(name);
        }

        public static global::Uno.Int4 GetInteger(GLInteger4Name name)
        {
            return GL._gl.GetInteger(name);
        }

        public static void Disable(GLEnableCap cap)
        {
            GL._gl.Disable(cap);
        }

        public static void Enable(GLEnableCap cap)
        {
            GL._gl.Enable(cap);
        }

        public static void Finish()
        {
            GL._gl.Finish();
        }

        public static void Flush()
        {
            GL._gl.Flush();
        }

        public static GLError GetError()
        {
            return GL._gl.GetError();
        }

        public static string GetString(GLStringName name)
        {
            return GL._gl.GetString(name);
        }

        public static void PixelStore(GLPixelStoreParameter pname, int param)
        {
            GL._gl.PixelStore(pname, param);
        }

        public static void Clear(GLClearBufferMask mask)
        {
            GL._gl.Clear(mask);
        }

        public static void ClearColor(float red, float green, float blue, float alpha)
        {
            GL._gl.ClearColor(red, green, blue, alpha);
        }

        public static void ClearDepth(float depth)
        {
            GL._gl.ClearDepth(depth);
        }

        public static void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            GL._gl.ColorMask(red, green, blue, alpha);
        }

        public static void DepthMask(bool flag)
        {
            GL._gl.DepthMask(flag);
        }

        public static void BlendEquation(GLBlendEquation mode)
        {
            GL._gl.BlendEquation(mode);
        }

        public static void BlendEquationSeparate(GLBlendEquation modeRgb, GLBlendEquation modeAlpha)
        {
            GL._gl.BlendEquationSeparate(modeRgb, modeAlpha);
        }

        public static void BlendFunc(GLBlendingFactor src, GLBlendingFactor dst)
        {
            GL._gl.BlendFunc(src, dst);
        }

        public static void BlendFuncSeparate(GLBlendingFactor srcRGB, GLBlendingFactor dstRGB, GLBlendingFactor srcAlpha, GLBlendingFactor dstAlpha)
        {
            GL._gl.BlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
        }

        public static void DepthFunc(GLDepthFunction func)
        {
            GL._gl.DepthFunc(func);
        }

        public static void CullFace(GLCullFaceMode mode)
        {
            GL._gl.CullFace(mode);
        }

        public static void FrontFace(GLFrontFaceDirection mode)
        {
            GL._gl.FrontFace(mode);
        }

        public static void LineWidth(float width)
        {
            GL._gl.LineWidth(width);
        }

        public static void PolygonOffset(float factor, float units)
        {
            GL._gl.PolygonOffset(factor, units);
        }

        public static void DepthRange(float zNear, float zFar)
        {
            GL._gl.DepthRange(zNear, zFar);
        }

        public static void Scissor(int x, int y, int width, int height)
        {
            GL._gl.Scissor(x, y, width, height);
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            GL._gl.Viewport(x, y, width, height);
        }

        public static void BindBuffer(GLBufferTarget target, GLBufferHandle buffer)
        {
            GL._gl.BindBuffer(target, buffer);
        }

        public static void BufferData(GLBufferTarget target, int sizeInBytes, global::System.IntPtr data, GLBufferUsage usage)
        {
            GL._gl.BufferData(target, sizeInBytes, data, usage);
        }

        public static void BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, global::System.IntPtr data)
        {
            GL._gl.BufferSubData(target, offset, sizeInBytes, data);
        }

        public static GLBufferHandle CreateBuffer()
        {
            return GL._gl.CreateBuffer();
        }

        public static void DeleteBuffer(GLBufferHandle buffer)
        {
            GL._gl.DeleteBuffer(buffer);
        }

        public static void BindFramebuffer(GLFramebufferTarget target, GLFramebufferHandle fb)
        {
            GL._gl.BindFramebuffer(target, fb);
        }

        public static GLFramebufferStatus CheckFramebufferStatus(GLFramebufferTarget target)
        {
            return GL._gl.CheckFramebufferStatus(target);
        }

        public static GLFramebufferHandle CreateFramebuffer()
        {
            return GL._gl.CreateFramebuffer();
        }

        public static void DeleteFramebuffer(GLFramebufferHandle fb)
        {
            GL._gl.DeleteFramebuffer(fb);
        }

        public static void FramebufferTexture2D(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLTextureTarget textarget, GLTextureHandle texture, int level)
        {
            GL._gl.FramebufferTexture2D(target, attachment, textarget, texture, level);
        }

        public static void FramebufferRenderbuffer(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLRenderbufferTarget renderbuffertarget, GLRenderbufferHandle renderbuffer)
        {
            GL._gl.FramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
        }

        public static GLFramebufferHandle GetFramebufferBinding()
        {
            return GL._gl.GetFramebufferBinding();
        }

        public static void BindRenderbuffer(GLRenderbufferTarget target, GLRenderbufferHandle renderbuffer)
        {
            GL._gl.BindRenderbuffer(target, renderbuffer);
        }

        public static GLRenderbufferHandle CreateRenderbuffer()
        {
            return GL._gl.CreateRenderbuffer();
        }

        public static void DeleteRenderbuffer(GLRenderbufferHandle renderbuffer)
        {
            GL._gl.DeleteRenderbuffer(renderbuffer);
        }

        public static void RenderbufferStorage(GLRenderbufferTarget target, GLRenderbufferStorage internalFormat, int width, int height)
        {
            GL._gl.RenderbufferStorage(target, internalFormat, width, height);
        }

        public static GLRenderbufferHandle GetRenderbufferBinding()
        {
            return GL._gl.GetRenderbufferBinding();
        }

        public static void ActiveTexture(GLTextureUnit texture)
        {
            GL._gl.ActiveTexture(texture);
        }

        public static void BindTexture(GLTextureTarget target, GLTextureHandle texture)
        {
            GL._gl.BindTexture(target, texture);
        }

        public static GLTextureHandle CreateTexture()
        {
            return GL._gl.CreateTexture();
        }

        public static void DeleteTexture(GLTextureHandle texture)
        {
            GL._gl.DeleteTexture(texture);
        }

        public static void GenerateMipmap(GLTextureTarget target)
        {
            GL._gl.GenerateMipmap(target);
        }

        public static void TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, global::System.IntPtr data)
        {
            GL._gl.TexImage2D(target, level, internalFormat, width, height, border, format, type, data);
        }

        public static void TexSubImage2D(GLTextureTarget target, int level, int xoffset, int yoffset, int width, int height, GLPixelFormat format, GLPixelType type, global::System.IntPtr data)
        {
            GL._gl.TexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, data);
        }

        public static void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureParameterValue param)
        {
            GL._gl.TexParameter(target, pname, param);
        }

        public static void AttachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            GL._gl.AttachShader(program, shader);
        }

        public static void BindAttribLocation(GLProgramHandle handle, int index, string name)
        {
            GL._gl.BindAttribLocation(handle, index, name);
        }

        public static void CompileShader(GLShaderHandle shader)
        {
            GL._gl.CompileShader(shader);
        }

        public static GLProgramHandle CreateProgram()
        {
            return GL._gl.CreateProgram();
        }

        public static GLShaderHandle CreateShader(GLShaderType type)
        {
            return GL._gl.CreateShader(type);
        }

        public static void DeleteProgram(GLProgramHandle program)
        {
            GL._gl.DeleteProgram(program);
        }

        public static void DeleteShader(GLShaderHandle shader)
        {
            GL._gl.DeleteShader(shader);
        }

        public static void DetachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            GL._gl.DetachShader(program, shader);
        }

        public static int GetProgramParameter(GLProgramHandle program, GLProgramParameter pname)
        {
            return GL._gl.GetProgramParameter(program, pname);
        }

        public static string GetProgramInfoLog(GLProgramHandle program)
        {
            return GL._gl.GetProgramInfoLog(program);
        }

        public static int GetShaderParameter(GLShaderHandle shader, GLShaderParameter pname)
        {
            return GL._gl.GetShaderParameter(shader, pname);
        }

        public static string GetShaderInfoLog(GLShaderHandle shader)
        {
            return GL._gl.GetShaderInfoLog(shader);
        }

        public static void LinkProgram(GLProgramHandle program)
        {
            GL._gl.LinkProgram(program);
        }

        public static void ShaderSource(GLShaderHandle shader, string source)
        {
            GL._gl.ShaderSource(shader, source);
        }

        public static void UseProgram(GLProgramHandle program)
        {
            GL._gl.UseProgram(program);
        }

        public static GLShaderPrecisionFormat GetShaderPrecisionFormat(GLShaderType shaderType, GLShaderPrecision precision)
        {
            return GL._gl.GetShaderPrecisionFormat(shaderType, precision);
        }

        public static void DisableVertexAttribArray(int index)
        {
            GL._gl.DisableVertexAttribArray(index);
        }

        public static void EnableVertexAttribArray(int index)
        {
            GL._gl.EnableVertexAttribArray(index);
        }

        public static int GetAttribLocation(GLProgramHandle program, string name)
        {
            return GL._gl.GetAttribLocation(program, name);
        }

        public static int GetUniformLocation(GLProgramHandle program, string name)
        {
            return GL._gl.GetUniformLocation(program, name);
        }

        public static void Uniform1(int location, int value)
        {
            GL._gl.Uniform1(location, value);
        }

        public static void Uniform2(int location, global::Uno.Int2 value)
        {
            GL._gl.Uniform2(location, value);
        }

        public static void Uniform3(int location, global::Uno.Int3 value)
        {
            GL._gl.Uniform3(location, value);
        }

        public static void Uniform4(int location, global::Uno.Int4 value)
        {
            GL._gl.Uniform4(location, value);
        }

        public static void Uniform1(int location, float value)
        {
            GL._gl.Uniform1(location, value);
        }

        public static void Uniform2(int location, global::Uno.Float2 value)
        {
            GL._gl.Uniform2(location, value);
        }

        public static void Uniform3(int location, global::Uno.Float3 value)
        {
            GL._gl.Uniform3(location, value);
        }

        public static void Uniform4(int location, global::Uno.Float4 value)
        {
            GL._gl.Uniform4(location, value);
        }

        public static void UniformMatrix2(int location, bool transpose, global::Uno.Float2x2 value)
        {
            GL._gl.UniformMatrix2(location, transpose, value);
        }

        public static void UniformMatrix3(int location, bool transpose, global::Uno.Float3x3 value)
        {
            GL._gl.UniformMatrix3(location, transpose, value);
        }

        public static void UniformMatrix4(int location, bool transpose, global::Uno.Float4x4 value)
        {
            GL._gl.UniformMatrix4(location, transpose, value);
        }

        public static void Uniform1(int location, int[] value)
        {
            GL._gl.Uniform1(location, value);
        }

        public static void Uniform2(int location, global::Uno.Int2[] value)
        {
            GL._gl.Uniform2(location, value);
        }

        public static void Uniform3(int location, global::Uno.Int3[] value)
        {
            GL._gl.Uniform3(location, value);
        }

        public static void Uniform4(int location, global::Uno.Int4[] value)
        {
            GL._gl.Uniform4(location, value);
        }

        public static void Uniform1(int location, float[] value)
        {
            GL._gl.Uniform1(location, value);
        }

        public static void Uniform2(int location, global::Uno.Float2[] value)
        {
            GL._gl.Uniform2(location, value);
        }

        public static void Uniform3(int location, global::Uno.Float3[] value)
        {
            GL._gl.Uniform3(location, value);
        }

        public static void Uniform4(int location, global::Uno.Float4[] value)
        {
            GL._gl.Uniform4(location, value);
        }

        public static void UniformMatrix2(int location, bool transpose, global::Uno.Float2x2[] value)
        {
            GL._gl.UniformMatrix2(location, transpose, value);
        }

        public static void UniformMatrix3(int location, bool transpose, global::Uno.Float3x3[] value)
        {
            GL._gl.UniformMatrix3(location, transpose, value);
        }

        public static void UniformMatrix4(int location, bool transpose, global::Uno.Float4x4[] value)
        {
            GL._gl.UniformMatrix4(location, transpose, value);
        }

        public static void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
        {
            GL._gl.VertexAttribPointer(index, size, type, normalized, stride, offset);
        }

        public static void DrawArrays(GLPrimitiveType mode, int first, int count)
        {
            GL._gl.DrawArrays(mode, first, count);
        }

        public static void DrawElements(GLPrimitiveType mode, int count, GLIndexType type, int offset)
        {
            GL._gl.DrawElements(mode, count, type, offset);
        }

        public static void ReadPixels(int x, int y, int width, int height, GLPixelFormat format, GLPixelType type, byte[] data)
        {
            GL._gl.ReadPixels(x, y, width, height, format, type, data);
        }

        public static void Initialize(IGL gl, bool debug)
        {
            GL._gl = debug ? (IGL)new GLDebugLayer(gl) : gl;
            string glVersion = gl.GetString(GLStringName.Version);

            if (glVersion.StartsWith("OpenGL ES"))
                return;

            if (((glVersion == null) || (glVersion.IndexOf('.') == -1)) || (int.Parse(glVersion.Substring(0, glVersion.IndexOf('.'))) < 2))
            {
                global::Uno.Diagnostics.Debug.Log("GL_VERSION: " + glVersion, global::Uno.Diagnostics.DebugMessageType.Debug);
                global::Uno.Diagnostics.Debug.Log("GL_VENDOR: " + gl.GetString(GLStringName.Vendor), global::Uno.Diagnostics.DebugMessageType.Debug);
                global::Uno.Diagnostics.Debug.Log("GL_RENDERER: " + gl.GetString(GLStringName.Renderer), global::Uno.Diagnostics.DebugMessageType.Debug);
                throw new global::System.NotSupportedException("OpenGL 2.0 is required to run this application");
            }
        }

        public static bool HasGetShaderPrecisionFormat
        {
            get
            {
                return GL._gl.HasGetShaderPrecisionFormat;
            }
        }
    }
}
