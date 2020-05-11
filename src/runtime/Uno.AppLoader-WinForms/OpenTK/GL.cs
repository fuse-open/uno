using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenGL;
using OpenTK.Graphics.ES20;
using TKGL = OpenTK.Graphics.ES20.GL;

namespace Uno.Support.OpenTK
{
    public class GL : IGL
    {
        // Workaround for InvalidProgramException
        GLFramebufferHandle _currentFramebufferBinding;
        GLRenderbufferHandle _currentRenderbufferBinding;

        readonly LinkedList<TextureDisposable> _textures = new LinkedList<TextureDisposable>();
        readonly LinkedList<FramebufferDisposable> _framebuffers = new LinkedList<FramebufferDisposable>();
        readonly LinkedList<BufferDisposable> _buffers = new LinkedList<BufferDisposable>();
        readonly LinkedList<RenderbufferDisposable> _renderbuffers = new LinkedList<RenderbufferDisposable>();
        readonly LinkedList<ShaderDisposable> _shaders = new LinkedList<ShaderDisposable>();
        readonly LinkedList<ProgramDisposable> _programs = new LinkedList<ProgramDisposable>();

        void AddContextObject(TextureDisposable obj) { _textures.AddLast(obj); }
        void AddContextObject(FramebufferDisposable obj) { _framebuffers.AddLast(obj); }
        void AddContextObject(BufferDisposable obj) { _buffers.AddLast(obj); }
        void AddContextObject(RenderbufferDisposable obj) { _renderbuffers.AddLast(obj); }
        void AddContextObject(ShaderDisposable obj) { _shaders.AddLast(obj); }
        void AddContextObject(ProgramDisposable obj) { _programs.AddLast(obj); }

        public void DisposeContext()
        {
            DisposeAndRemoveObjects(_textures);
            DisposeAndRemoveObjects(_framebuffers);
            DisposeAndRemoveObjects(_buffers);
            DisposeAndRemoveObjects(_renderbuffers);
            DisposeAndRemoveObjects(_shaders);
            DisposeAndRemoveObjects(_programs);
        }

        void DisposeAndRemoveObject<T>(LinkedList<T> objects, int handle) where T : IContextObjectDisposable
        {
            var obj = objects.FirstOrDefault(o => o.HandleName == handle);
            if (obj == null) throw new Exception("Trying to dispose object not created by this instance of IGL");
            obj.Dispose();
            objects.Remove(obj);
        }


        void DisposeAndRemoveObjects<T>(LinkedList<T> objects) where T : IContextObjectDisposable
        {
            foreach (var obj in objects) obj.Dispose();
            objects.Clear();
        }

        public GLError GetError()
        {
            return (GLError)TKGL.GetError();
        }

        public void Finish()
        {
            TKGL.Finish();
        }

        public void Flush()
        {
            TKGL.Flush();
        }

        public int GetInteger(GLIntegerName pname)
        {
            return TKGL.GetInteger((GetPName)pname);
        }

        public Int4 GetInteger(GLInteger4Name pname)
        {
            var p = new int[4];
            TKGL.GetInteger((GetPName)pname, p);
            return new Int4(p[0], p[1], p[2], p[3]);
        }

        public GLFramebufferStatus CheckFramebufferStatus(GLFramebufferTarget target)
        {
            return (GLFramebufferStatus)TKGL.CheckFramebufferStatus((FramebufferTarget)target);
        }

        public void Clear(GLClearBufferMask mask)
        {
            TKGL.Clear((ClearBufferMask)mask);
        }

        public void ClearColor(float red, float green, float blue, float alpha)
        {
            TKGL.ClearColor(red, green, blue, alpha);
        }

        public void ClearDepth(float depth)
        {
            TKGL.ClearDepth(depth);
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            TKGL.ColorMask(red, green, blue, alpha);
        }

        public void DepthMask(bool flag)
        {
            TKGL.DepthMask(flag);
        }

        public GLTextureHandle CreateTexture()
        {
            int texture = TKGL.GenTexture();
            AddContextObject(new TextureDisposable(texture));
            return new GLTextureHandle(texture);
        }

        public void DeleteTexture(GLTextureHandle texture)
        {
            DisposeAndRemoveObject(_textures, (int) texture);
        }

        public void ActiveTexture(GLTextureUnit texture)
        {
            TKGL.ActiveTexture((TextureUnit)texture);
        }

        public void BindTexture(GLTextureTarget target, GLTextureHandle texture)
        {
            TKGL.BindTexture((TextureTarget)target, (int) texture);
        }

        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureParameterValue param)
        {
            TKGL.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);
        }

        public void TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, IntPtr data)
        {
            TKGL.TexImage2D((TextureTarget2d)target, level,
                    (TextureComponentCount)internalFormat, width, height, border,
                    (PixelFormat)format, (PixelType)type,
                    data);
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
            TKGL.TexSubImage2D((TextureTarget2d)target, level,
                            xoffset, yoffset, width, height,
                            (PixelFormat)format, (PixelType)type,
                            data);
        }

        public void GenerateMipmap(GLTextureTarget target)
        {
            TKGL.GenerateMipmap((TextureTarget)target);
        }

        public void PixelStore(GLPixelStoreParameter pname, int param)
        {
            TKGL.PixelStore((PixelStoreParameter)pname, param);
        }

        public GLRenderbufferHandle CreateRenderbuffer()
        {
            int r;
            TKGL.GenRenderbuffers(1, out r);
            AddContextObject(new RenderbufferDisposable(r));
            return new GLRenderbufferHandle(r);
        }

        public void DeleteRenderbuffer(GLRenderbufferHandle renderbuffer)
        {
            DisposeAndRemoveObject(_renderbuffers, (int) renderbuffer);
        }

        public void BindRenderbuffer(GLRenderbufferTarget target, GLRenderbufferHandle renderbuffer)
        {
            _currentRenderbufferBinding = renderbuffer;
            TKGL.BindRenderbuffer((RenderbufferTarget)target, (int) renderbuffer);
        }

        public void RenderbufferStorage(GLRenderbufferTarget target, GLRenderbufferStorage internalFormat, int width, int height)
        {
            TKGL.RenderbufferStorage((RenderbufferTarget)target, (RenderbufferInternalFormat)internalFormat, width, height);
        }

        public GLFramebufferHandle CreateFramebuffer()
        {
            int r;
            TKGL.GenFramebuffers(1, out r);
            AddContextObject(new FramebufferDisposable(r));
            return new GLFramebufferHandle(r);
        }

        public void DeleteFramebuffer(GLFramebufferHandle fb)
        {
            DisposeAndRemoveObject(_framebuffers, (int) fb);
        }

        public void BindFramebuffer(GLFramebufferTarget target, GLFramebufferHandle fb)
        {
            _currentFramebufferBinding = fb;
            TKGL.BindFramebuffer((FramebufferTarget)target, (int) fb);
        }

        public void FramebufferTexture2D(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLTextureTarget textarget, GLTextureHandle texture, int level)
        {
            TKGL.FramebufferTexture2D((FramebufferTarget)target, (FramebufferAttachment)attachment, TextureTarget2d.Texture2D, (int)texture, level);
        }

        public void FramebufferRenderbuffer(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLRenderbufferTarget renderbuffertarget, GLRenderbufferHandle renderbuffer)
        {
            TKGL.FramebufferRenderbuffer((FramebufferTarget)target,
                (FramebufferAttachment)attachment,
                (RenderbufferTarget)renderbuffertarget, (int) renderbuffer);
        }

        public void UseProgram(GLProgramHandle program)
        {
            TKGL.UseProgram((int) program);
        }

        public void BindAttribLocation(GLProgramHandle program, int index, string name)
        {
            TKGL.BindAttribLocation((int) program, index, name);
        }

        public int GetAttribLocation(GLProgramHandle program, string name)
        {
            return TKGL.GetAttribLocation((int) program, name);
        }

        public int GetUniformLocation(GLProgramHandle program, string name)
        {
            return TKGL.GetUniformLocation((int) program, name);
        }

        public void Uniform1(int location, int value)
        {
            TKGL.Uniform1(location, value);
        }

        public void Uniform2(int location, Int2 value)
        {
            TKGL.Uniform2(location, value.X, value.Y);
        }

        public void Uniform3(int location, Int3 value)
        {
            TKGL.Uniform3(location, value.X, value.Y, value.Z);
        }

        public void Uniform4(int location, Int4 value)
        {
            TKGL.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }

        public void Uniform1(int location, float value)
        {
            TKGL.Uniform1(location, value);
        }

        public void Uniform2(int location, Float2 value)
        {
            TKGL.Uniform2(location, value.X, value.Y);
        }

        public void Uniform3(int location, Float3 value)
        {
            TKGL.Uniform3(location, value.X, value.Y, value.Z);
        }

        public void Uniform4(int location, Float4 value)
        {
            TKGL.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }

        public void UniformMatrix2(int location, bool transpose, Float2x2 value)
        {
            TKGL.UniformMatrix2(location, 1, transpose, ref value.M11);
        }

        public void UniformMatrix3(int location, bool transpose, Float3x3 value)
        {
            TKGL.UniformMatrix3(location, 1, transpose, ref value.M11);
        }

        public void UniformMatrix4(int location, bool transpose, Float4x4 value)
        {
            // Who is always changing this function creating a temp array? Please stop committing your workarounds. Thanks
            TKGL.UniformMatrix4(location, 1, transpose, ref value.M11);
        }

        public void Uniform1(int location, int[] value)
        {
            TKGL.Uniform1(location, value.Length, value);
        }

        public void Uniform2(int location, Int2[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (int* p = &value[0].X)
                        TKGL.Uniform2(location, value.Length, p);
                }
            }
        }

        public void Uniform3(int location, Int3[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (int* p = &value[0].X)
                        TKGL.Uniform3(location, value.Length, p);
                }
            }
        }

        public void Uniform4(int location, Int4[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (int* p = &value[0].X)
                        TKGL.Uniform4(location, value.Length, p);
                }
            }
        }

        public void Uniform1(int location, float[] value)
        {
            TKGL.Uniform1(location, value.Length, value);
        }

        public void Uniform2(int location, Float2[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (float* p = &value[0].X)
                        TKGL.Uniform2(location, value.Length, p);
                }
            }
        }

        public void Uniform3(int location, Float3[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (float* p = &value[0].X)
                        TKGL.Uniform3(location, value.Length, p);
                }
            }
        }

        public void Uniform4(int location, Float4[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (float* p = &value[0].X)
                        TKGL.Uniform4(location, value.Length, p);
                }
            }
        }

        public void UniformMatrix2(int location, bool transpose, Float2x2[] value)
        {
            if (value.Length > 0)
                TKGL.UniformMatrix2(location, value.Length, transpose, ref value[0].M11);
        }

        public void UniformMatrix3(int location, bool transpose, Float3x3[] value)
        {
            if (value.Length > 0)
                TKGL.UniformMatrix3(location, value.Length, transpose, ref value[0].M11);
        }

        public void UniformMatrix4(int location, bool transpose, Float4x4[] value)
        {
            if (value.Length > 0)
                TKGL.UniformMatrix4(location, value.Length, transpose, ref value[0].M11);
        }

        public void EnableVertexAttribArray(int index)
        {
            TKGL.EnableVertexAttribArray(index);
        }

        public void DisableVertexAttribArray(int index)
        {
            TKGL.DisableVertexAttribArray(index);
        }

        public void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
        {
            TKGL.VertexAttribPointer(index, size, (VertexAttribPointerType)type, normalized, stride, offset);
        }

        public void DrawArrays(GLPrimitiveType mode, int first, int count)
        {
            TKGL.DrawArrays((PrimitiveType)mode, first, count);
        }

        public void DrawElements(GLPrimitiveType mode, int count, GLIndexType type, int offset)
        {
            TKGL.DrawElements((BeginMode) mode, count, (DrawElementsType)type, offset);
        }

        public GLBufferHandle CreateBuffer()
        {
            int r;
            TKGL.GenBuffers(1, out r);
            AddContextObject(new BufferDisposable(r));
            return new GLBufferHandle(r);
        }

        public void DeleteBuffer(GLBufferHandle buffer)
        {
            DisposeAndRemoveObject(_buffers, (int) buffer);
        }

        public void BindBuffer(GLBufferTarget target, GLBufferHandle buffer)
        {
            TKGL.BindBuffer((BufferTarget)target, (int) buffer);
        }

        public void BufferData(GLBufferTarget target, int sizeInBytes, IntPtr data, GLBufferUsage usage)
        {
            TKGL.BufferData((BufferTarget)target, (IntPtr)sizeInBytes, data, (BufferUsageHint)usage);
        }

        public void BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, IntPtr data)
        {
            TKGL.BufferSubData((BufferTarget)target, (IntPtr)offset, sizeInBytes, data);
        }

        public void Enable(GLEnableCap cap)
        {
            TKGL.Enable((EnableCap)cap);
        }

        public void Disable(GLEnableCap cap)
        {
            TKGL.Disable((EnableCap)cap);
        }

        public bool IsEnabled(GLEnableCap cap)
        {
            return TKGL.IsEnabled((EnableCap)cap);
        }

        public void BlendFunc(GLBlendingFactor src, GLBlendingFactor dst)
        {
            TKGL.BlendFunc((BlendingFactorSrc)src, (BlendingFactorDest)dst);
        }

        public void BlendFuncSeparate(GLBlendingFactor srcRGB, GLBlendingFactor dstRGB, GLBlendingFactor srcAlpha, GLBlendingFactor dstAlpha)
        {
            TKGL.BlendFuncSeparate((BlendingFactorSrc)srcRGB, (BlendingFactorDest)dstRGB,
                (BlendingFactorSrc)srcAlpha, (BlendingFactorDest)dstAlpha);
        }

        public void BlendEquation(GLBlendEquation mode)
        {
            TKGL.BlendEquation((BlendEquationMode)mode);
        }

        public void BlendEquationSeparate(GLBlendEquation modeRgb, GLBlendEquation modeAlpha)
        {
            TKGL.BlendEquationSeparate((BlendEquationMode)modeRgb, (BlendEquationMode)modeAlpha);
        }

        public void CullFace(GLCullFaceMode mode)
        {
            TKGL.CullFace((CullFaceMode)mode);
        }

        public void FrontFace(GLFrontFaceDirection mode)
        {
            TKGL.FrontFace((FrontFaceDirection)mode);
        }

        public void DepthFunc(GLDepthFunction func)
        {
            TKGL.DepthFunc((DepthFunction)func);
        }

        public void Scissor(int x, int y, int width, int height)
        {
            TKGL.Scissor(x, y, width, height);
        }

        public void Viewport(int x, int y, int width, int height)
        {
            TKGL.Viewport(x, y, width, height);
        }

        public void LineWidth(float width)
        {
            TKGL.LineWidth(width);
        }

        public void PointSize(float size)
        {
            // TODO: Remove method from GL interface.
            // glPointSize isn't supported by OpenGL ES 2.0, instead use the built-in vertex shader variable 'gl_PointSize'.
        }

        public void PolygonOffset(float factor, float units)
        {
            TKGL.PolygonOffset(factor, units);
        }

        public void DepthRange(float zNear, float zFar)
        {
            TKGL.DepthRange(zNear, zFar);
        }

        public GLShaderHandle CreateShader(GLShaderType type)
        {
            int shaderName = TKGL.CreateShader((ShaderType)type);
            AddContextObject(new ShaderDisposable(shaderName));
            return new GLShaderHandle(shaderName);
        }

        public void DeleteShader(GLShaderHandle shader)
        {
            DisposeAndRemoveObject(_shaders, (int) shader);
        }

        public void ShaderSource(GLShaderHandle shader, string source)
        {
            TKGL.ShaderSource((int)shader, source);
        }

        public void ReadPixels(int x, int y, int width, int height, GLPixelFormat format, GLPixelType type, byte[] buffer)
        {
            TKGL.ReadPixels(x, y, width, height, (PixelFormat)format, (PixelType)type, buffer);
        }

        public void CompileShader(GLShaderHandle shader)
        {
            TKGL.CompileShader((int) shader);
        }

        public int GetShaderParameter(GLShaderHandle shader, GLShaderParameter pname)
        {
            int result;
            TKGL.GetShader((uint) (int) shader, (ShaderParameter)pname, out result);
            return result;
        }

        public string GetShaderInfoLog(GLShaderHandle shader)
        {
            return TKGL.GetShaderInfoLog((int) shader);
        }

        public GLProgramHandle CreateProgram()
        {
            int programName = TKGL.CreateProgram();
            AddContextObject(new ProgramDisposable(programName));
            return new GLProgramHandle(programName);
        }

        public void DeleteProgram(GLProgramHandle program)
        {
            DisposeAndRemoveObject(_programs, (int) program);
        }

        public void AttachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            TKGL.AttachShader((int) program, (int) shader);
        }

        public void DetachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            TKGL.DetachShader((int) program, (int) shader);
        }

        public void LinkProgram(GLProgramHandle program)
        {
            TKGL.LinkProgram((int) program);
        }

        public int GetProgramParameter(GLProgramHandle program, GLProgramParameter pname)
        {
            int result;
            TKGL.GetProgram((uint) (int) program, (GetProgramParameterName)pname, out result);
            return result;
        }

        public string GetProgramInfoLog(GLProgramHandle program)
        {
            return TKGL.GetProgramInfoLog((int) program);
        }

        public bool HasGetShaderPrecisionFormat => false;

        public GLShaderPrecisionFormat GetShaderPrecisionFormat(GLShaderType shaderType, GLShaderPrecision precision)
        {
            return new GLShaderPrecisionFormat();
        }

        public string GetString(GLStringName name)
        {
            return TKGL.GetString((StringName)name);
        }

        public GLRenderbufferHandle GetRenderbufferBinding()
        {
            return _currentRenderbufferBinding;
        }

        public GLFramebufferHandle GetFramebufferBinding()
        {
            return _currentFramebufferBinding;
        }
    }
}
