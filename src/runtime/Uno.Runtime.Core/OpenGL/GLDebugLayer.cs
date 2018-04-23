// This file was generated based on Library/Core/UnoCore/Source/OpenGL/GLDebugLayer.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace OpenGL
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public class GLDebugLayer : IGL
    {
        public readonly IGL _gl;

        public GLDebugLayer(IGL gl)
        {
            this._gl = gl;
        }

        public GLError GetError()
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetError();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Finish()
        {
            try
            {
                this.BeginFunc();
                this._gl.Finish();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Flush()
        {
            try
            {
                this.BeginFunc();
                this._gl.Flush();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLFramebufferStatus CheckFramebufferStatus(GLFramebufferTarget target)
        {
            try
            {
                this.BeginFunc();
                return this._gl.CheckFramebufferStatus(target);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public int GetInteger(GLIntegerName name)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetInteger(name);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public global::Uno.Int4 GetInteger(GLInteger4Name name)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetInteger(name);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public string GetString(GLStringName name)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetString(name);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Clear(GLClearBufferMask mask)
        {
            try
            {
                this.BeginFunc();
                this._gl.Clear(mask);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void ClearColor(float red, float green, float blue, float alpha)
        {
            try
            {
                this.BeginFunc();
                this._gl.ClearColor(red, green, blue, alpha);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void ClearDepth(float depth)
        {
            try
            {
                this.BeginFunc();
                this._gl.ClearDepth(depth);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            try
            {
                this.BeginFunc();
                this._gl.ColorMask(red, green, blue, alpha);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DepthMask(bool flag)
        {
            try
            {
                this.BeginFunc();
                this._gl.DepthMask(flag);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLTextureHandle CreateTexture()
        {
            try
            {
                this.BeginFunc();
                return this._gl.CreateTexture();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DeleteTexture(GLTextureHandle texture)
        {
            try
            {
                this.BeginFunc();
                this._gl.DeleteTexture(texture);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void ActiveTexture(GLTextureUnit texture)
        {
            try
            {
                this.BeginFunc();
                this._gl.ActiveTexture(texture);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BindTexture(GLTextureTarget target, GLTextureHandle texture)
        {
            try
            {
                this.BeginFunc();
                this._gl.BindTexture(target, texture);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureParameterValue param)
        {
            try
            {
                this.BeginFunc();
                this._gl.TexParameter(target, pname, param);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, global::System.IntPtr data)
        {
            try
            {
                this.BeginFunc();
                this._gl.TexImage2D(target, level, internalFormat, width, height, border, format, type, data);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void TexSubImage2D(GLTextureTarget target, int level, int xoffset, int yoffset, int width, int height, GLPixelFormat format, GLPixelType type, global::System.IntPtr data)
        {
            try
            {
                this.BeginFunc();
                this._gl.TexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, data);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void GenerateMipmap(GLTextureTarget target)
        {
            try
            {
                this.BeginFunc();
                this._gl.GenerateMipmap(target);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void PixelStore(GLPixelStoreParameter pname, int param)
        {
            try
            {
                this.BeginFunc();
                this._gl.PixelStore(pname, param);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void ReadPixels(int x, int y, int width, int height, GLPixelFormat format, GLPixelType type, byte[] buffer)
        {
            try
            {
                this.BeginFunc();
                this._gl.ReadPixels(x, y, width, height, format, type, buffer);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLRenderbufferHandle CreateRenderbuffer()
        {
            try
            {
                this.BeginFunc();
                return this._gl.CreateRenderbuffer();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DeleteRenderbuffer(GLRenderbufferHandle renderbuffer)
        {
            try
            {
                this.BeginFunc();
                this._gl.DeleteRenderbuffer(renderbuffer);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BindRenderbuffer(GLRenderbufferTarget target, GLRenderbufferHandle renderbuffer)
        {
            try
            {
                this.BeginFunc();
                this._gl.BindRenderbuffer(target, renderbuffer);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void RenderbufferStorage(GLRenderbufferTarget target, GLRenderbufferStorage internalFormat, int width, int height)
        {
            try
            {
                this.BeginFunc();
                this._gl.RenderbufferStorage(target, internalFormat, width, height);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLRenderbufferHandle GetRenderbufferBinding()
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetRenderbufferBinding();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLFramebufferHandle CreateFramebuffer()
        {
            try
            {
                this.BeginFunc();
                return this._gl.CreateFramebuffer();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DeleteFramebuffer(GLFramebufferHandle fb)
        {
            try
            {
                this.BeginFunc();
                this._gl.DeleteFramebuffer(fb);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BindFramebuffer(GLFramebufferTarget target, GLFramebufferHandle fb)
        {
            try
            {
                this.BeginFunc();
                this._gl.BindFramebuffer(target, fb);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void FramebufferTexture2D(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLTextureTarget textarget, GLTextureHandle texture, int level)
        {
            try
            {
                this.BeginFunc();
                this._gl.FramebufferTexture2D(target, attachment, textarget, texture, level);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void FramebufferRenderbuffer(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLRenderbufferTarget renderbuffertarget, GLRenderbufferHandle renderbuffer)
        {
            try
            {
                this.BeginFunc();
                this._gl.FramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLFramebufferHandle GetFramebufferBinding()
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetFramebufferBinding();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void UseProgram(GLProgramHandle program)
        {
            try
            {
                this.BeginFunc();
                this._gl.UseProgram(program);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BindAttribLocation(GLProgramHandle program, int index, string name)
        {
            try
            {
                this.BeginFunc();
                this._gl.BindAttribLocation(program, index, name);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public int GetAttribLocation(GLProgramHandle program, string name)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetAttribLocation(program, name);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public int GetUniformLocation(GLProgramHandle program, string name)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetUniformLocation(program, name);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform1(int location, int value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform1(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform2(int location, global::Uno.Int2 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform2(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform3(int location, global::Uno.Int3 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform3(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform4(int location, global::Uno.Int4 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform4(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform1(int location, float value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform1(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform2(int location, global::Uno.Float2 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform2(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform3(int location, global::Uno.Float3 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform3(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform4(int location, global::Uno.Float4 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform4(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void UniformMatrix2(int location, bool transpose, global::Uno.Float2x2 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.UniformMatrix2(location, transpose, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void UniformMatrix3(int location, bool transpose, global::Uno.Float3x3 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.UniformMatrix3(location, transpose, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void UniformMatrix4(int location, bool transpose, global::Uno.Float4x4 value)
        {
            try
            {
                this.BeginFunc();
                this._gl.UniformMatrix4(location, transpose, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform1(int location, int[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform1(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform2(int location, global::Uno.Int2[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform2(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform3(int location, global::Uno.Int3[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform3(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform4(int location, global::Uno.Int4[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform4(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform1(int location, float[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform1(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform2(int location, global::Uno.Float2[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform2(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform3(int location, global::Uno.Float3[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform3(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Uniform4(int location, global::Uno.Float4[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.Uniform4(location, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void UniformMatrix2(int location, bool transpose, global::Uno.Float2x2[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.UniformMatrix2(location, transpose, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void UniformMatrix3(int location, bool transpose, global::Uno.Float3x3[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.UniformMatrix3(location, transpose, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void UniformMatrix4(int location, bool transpose, global::Uno.Float4x4[] value)
        {
            try
            {
                this.BeginFunc();
                this._gl.UniformMatrix4(location, transpose, value);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void EnableVertexAttribArray(int index)
        {
            try
            {
                this.BeginFunc();
                this._gl.EnableVertexAttribArray(index);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DisableVertexAttribArray(int index)
        {
            try
            {
                this.BeginFunc();
                this._gl.DisableVertexAttribArray(index);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
        {
            try
            {
                this.BeginFunc();
                this._gl.VertexAttribPointer(index, size, type, normalized, stride, offset);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DrawArrays(GLPrimitiveType mode, int first, int count)
        {
            try
            {
                this.BeginFunc();
                this._gl.DrawArrays(mode, first, count);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DrawElements(GLPrimitiveType mode, int count, GLIndexType type, int offset)
        {
            try
            {
                this.BeginFunc();
                this._gl.DrawElements(mode, count, type, offset);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLBufferHandle CreateBuffer()
        {
            try
            {
                this.BeginFunc();
                return this._gl.CreateBuffer();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DeleteBuffer(GLBufferHandle buffer)
        {
            try
            {
                this.BeginFunc();
                this._gl.DeleteBuffer(buffer);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BindBuffer(GLBufferTarget target, GLBufferHandle buffer)
        {
            try
            {
                this.BeginFunc();
                this._gl.BindBuffer(target, buffer);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BufferData(GLBufferTarget target, int sizeInBytes, global::System.IntPtr data, GLBufferUsage usage)
        {
            try
            {
                this.BeginFunc();
                this._gl.BufferData(target, sizeInBytes, data, usage);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, global::System.IntPtr data)
        {
            try
            {
                this.BeginFunc();
                this._gl.BufferSubData(target, offset, sizeInBytes, data);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Enable(GLEnableCap cap)
        {
            try
            {
                this.BeginFunc();
                this._gl.Enable(cap);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Disable(GLEnableCap cap)
        {
            try
            {
                this.BeginFunc();
                this._gl.Disable(cap);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BlendFunc(GLBlendingFactor src, GLBlendingFactor dst)
        {
            try
            {
                this.BeginFunc();
                this._gl.BlendFunc(src, dst);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BlendFuncSeparate(GLBlendingFactor srcRGB, GLBlendingFactor dstRGB, GLBlendingFactor srcAlpha, GLBlendingFactor dstAlpha)
        {
            try
            {
                this.BeginFunc();
                this._gl.BlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BlendEquation(GLBlendEquation mode)
        {
            try
            {
                this.BeginFunc();
                this._gl.BlendEquation(mode);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void BlendEquationSeparate(GLBlendEquation modeRgb, GLBlendEquation modeAlpha)
        {
            try
            {
                this.BeginFunc();
                this._gl.BlendEquationSeparate(modeRgb, modeAlpha);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void CullFace(GLCullFaceMode mode)
        {
            try
            {
                this.BeginFunc();
                this._gl.CullFace(mode);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void FrontFace(GLFrontFaceDirection mode)
        {
            try
            {
                this.BeginFunc();
                this._gl.FrontFace(mode);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DepthFunc(GLDepthFunction func)
        {
            try
            {
                this.BeginFunc();
                this._gl.DepthFunc(func);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Scissor(int x, int y, int width, int height)
        {
            try
            {
                this.BeginFunc();
                this._gl.Scissor(x, y, width, height);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void Viewport(int x, int y, int width, int height)
        {
            try
            {
                this.BeginFunc();
                this._gl.Viewport(x, y, width, height);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void LineWidth(float width)
        {
            try
            {
                this.BeginFunc();
                this._gl.LineWidth(width);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void PolygonOffset(float factor, float units)
        {
            try
            {
                this.BeginFunc();
                this._gl.PolygonOffset(factor, units);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DepthRange(float zNear, float zFar)
        {
            try
            {
                this.BeginFunc();
                this._gl.DepthRange(zNear, zFar);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLShaderHandle CreateShader(GLShaderType type)
        {
            try
            {
                this.BeginFunc();
                return this._gl.CreateShader(type);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DeleteShader(GLShaderHandle shader)
        {
            try
            {
                this.BeginFunc();
                this._gl.DeleteShader(shader);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void ShaderSource(GLShaderHandle shader, string source)
        {
            try
            {
                this.BeginFunc();
                this._gl.ShaderSource(shader, source);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void CompileShader(GLShaderHandle shader)
        {
            try
            {
                this.BeginFunc();
                this._gl.CompileShader(shader);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public int GetShaderParameter(GLShaderHandle shader, GLShaderParameter pname)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetShaderParameter(shader, pname);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public string GetShaderInfoLog(GLShaderHandle shader)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetShaderInfoLog(shader);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLProgramHandle CreateProgram()
        {
            try
            {
                this.BeginFunc();
                return this._gl.CreateProgram();
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DeleteProgram(GLProgramHandle program)
        {
            try
            {
                this.BeginFunc();
                this._gl.DeleteProgram(program);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void AttachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            try
            {
                this.BeginFunc();
                this._gl.AttachShader(program, shader);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void DetachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            try
            {
                this.BeginFunc();
                this._gl.DetachShader(program, shader);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void LinkProgram(GLProgramHandle program)
        {
            try
            {
                this.BeginFunc();
                this._gl.LinkProgram(program);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public int GetProgramParameter(GLProgramHandle program, GLProgramParameter pname)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetProgramParameter(program, pname);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public string GetProgramInfoLog(GLProgramHandle program)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetProgramInfoLog(program);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public GLShaderPrecisionFormat GetShaderPrecisionFormat(GLShaderType shader, GLShaderPrecision precision)
        {
            try
            {
                this.BeginFunc();
                return this._gl.GetShaderPrecisionFormat(shader, precision);
            }
            finally
            {
                this.EndFunc();
            }
        }

        public void CheckError()
        {
            GLError err = this._gl.GetError();

            if (err != GLError.NoError)
            {
                global::Uno.Diagnostics.Debug.Log(("GL error (" + (object)err) + ")", global::Uno.Diagnostics.DebugMessageType.Error);
                global::System.Diagnostics.StackFrame[] frames = new global::System.Diagnostics.StackTrace().GetFrames();

                for (int i = 3; i < frames.Length; i++)
                {
                    global::System.Diagnostics.StackFrame f = frames[i];

                    if (f.GetMethod().Name == "Main")
                        break;

                    global::Uno.Diagnostics.Debug.Log(((("in " + f.GetMethod().DeclaringType) + ".") + f.GetMethod().Name) + "()", global::Uno.Diagnostics.DebugMessageType.Debug);
                }
            }
        }

        public void BeginFunc()
        {
            this.CheckError();
        }

        public void EndFunc()
        {
            this.CheckError();
        }

        public bool HasGetShaderPrecisionFormat
        {
            get { return this._gl.HasGetShaderPrecisionFormat; }
        }
    }
}
