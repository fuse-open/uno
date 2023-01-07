using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using OpenGL;
using OpenTK.Graphics.OpenGL;
using MacGL = OpenTK.Graphics.OpenGL.GL;

namespace Uno.AppLoader.MonoMac
{
    [SupportedOSPlatform("macOS10.14")]
    public class MonoMacGL : IGL
    {
        internal const string Library = "/System/Library/Frameworks/OpenGL.framework/OpenGL";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Library, EntryPoint = "glGenerateMipmap", ExactSpelling = true)]
        internal extern static void glGenerateMipmap(GenerateMipmapTarget target);

        // Note: Workaround for InvalidProgramException
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

        public MonoMacGL()
        {
            MacGL.Enable(EnableCap.VertexProgramPointSize);
            MacGL.Enable(EnableCap.Texture2D);
            MacGL.Enable(EnableCap.TextureCubeMap);
        }

        public GLError GetError()
        {
            return (GLError)MacGL.GetError();
        }

        public void Finish()
        {
            MacGL.Finish();
        }

        public void Flush()
        {
            MacGL.Flush();
        }

        public GLFramebufferStatus CheckFramebufferStatus(GLFramebufferTarget target)
        {
            return (GLFramebufferStatus)MacGL.CheckFramebufferStatus((FramebufferTarget)target);
        }

        public int GetInteger(GLIntegerName pname)
        {
            int result;
            MacGL.GetInteger((GetPName)pname, out result);
            return result;
        }

        public Int4 GetInteger(GLInteger4Name pname)
        {
            var p = new int[4];
            MacGL.GetInteger((GetPName)pname, p);
            return new Int4(p[0], p[1], p[2], p[3]);
        }

        public void Clear(GLClearBufferMask mask)
        {
            MacGL.Clear((ClearBufferMask)mask);
        }

        public void ClearColor(float red, float green, float blue, float alpha)
        {
            MacGL.ClearColor(red, green, blue, alpha);
        }

        public void ClearDepth(float depth)
        {
            MacGL.ClearDepth(depth);
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            MacGL.ColorMask(red, green, blue, alpha);
        }

        public void DepthMask(bool flag)
        {
            MacGL.DepthMask(flag);
        }

        public GLTextureHandle CreateTexture()
        {
            var texture = MacGL.GenTexture();
            AddContextObject(new TextureDisposable (texture));
            return new GLTextureHandle(texture);
        }

        public void DeleteTexture(GLTextureHandle texture)
        {
            DisposeAndRemoveObject(_textures, (int) texture);
        }

        public void ActiveTexture(GLTextureUnit texture)
        {
            MacGL.ActiveTexture((TextureUnit)texture);
        }

        public void BindTexture(GLTextureTarget target, GLTextureHandle texture)
        {
            MacGL.BindTexture((TextureTarget)target, (int) texture);
        }

        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureParameterValue param)
        {
            MacGL.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);
        }

        public void TexImage2D(GLTextureTarget target, int level, GLPixelFormat internalFormat, int width, int height, int border, GLPixelFormat format, GLPixelType type, IntPtr data)
        {
            MacGL.TexImage2D((TextureTarget)target, level,
                                         (PixelInternalFormat)internalFormat, width, height, border,
                                         (PixelFormat)format, (PixelType)type,
                                         data);
        }

        public void TexSubImage2D(GLTextureTarget target, int level, int xoffset, int yoffset, int width, int height, GLPixelFormat format, GLPixelType type, IntPtr data)
        {
            MacGL.TexSubImage2D((TextureTarget)target, level,
                            xoffset, yoffset, width, height,
                            (PixelFormat)format, (PixelType)type,
                            data);
        }

        public void GenerateMipmap(GLTextureTarget target)
        {
            glGenerateMipmap((GenerateMipmapTarget)target);
        }

        public void PixelStore(GLPixelStoreParameter pname, int param)
        {
            MacGL.PixelStore((PixelStoreParameter)pname, param);
        }

        public GLRenderbufferHandle CreateRenderbuffer()
        {
            int r;
            MacGL.GenRenderbuffers(1, out r);
            AddContextObject(new RenderbufferDisposable (r));
            return new GLRenderbufferHandle(r);
        }

        public void DeleteRenderbuffer(GLRenderbufferHandle renderbuffer)
        {
            DisposeAndRemoveObject(_renderbuffers, (int) renderbuffer);
        }

        public void BindRenderbuffer(GLRenderbufferTarget target, GLRenderbufferHandle renderbuffer)
        {
            _currentRenderbufferBinding = renderbuffer;
            MacGL.BindRenderbuffer((RenderbufferTarget)target, (int) renderbuffer);
        }

        public void RenderbufferStorage(GLRenderbufferTarget target, GLRenderbufferStorage internalFormat, int width, int height)
        {
            MacGL.RenderbufferStorage((RenderbufferTarget)target, (RenderbufferStorage)internalFormat, width, height);
        }

        public GLFramebufferHandle CreateFramebuffer()
        {
            int r;
            MacGL.GenFramebuffers(1, out r);
            AddContextObject(new FramebufferDisposable (r));
            return new GLFramebufferHandle(r);
        }

        public void DeleteFramebuffer(GLFramebufferHandle fb)
        {
            DisposeAndRemoveObject(_framebuffers, (int) fb);
        }

        public void BindFramebuffer(GLFramebufferTarget target, GLFramebufferHandle fb)
        {
            _currentFramebufferBinding = fb;
            MacGL.BindFramebuffer((FramebufferTarget)target, (int) fb);
        }

        public void FramebufferTexture2D(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLTextureTarget textarget, GLTextureHandle texture, int level)
        {
            MacGL.FramebufferTexture2D((FramebufferTarget)target,
                (FramebufferAttachment)attachment,
                (TextureTarget)textarget, (int) texture, level);
        }

        public void FramebufferRenderbuffer(GLFramebufferTarget target, GLFramebufferAttachment attachment, GLRenderbufferTarget renderbuffertarget, GLRenderbufferHandle renderbuffer)
        {
            MacGL.FramebufferRenderbuffer((FramebufferTarget)target,
                (FramebufferAttachment)attachment,
                (RenderbufferTarget)renderbuffertarget, (int) renderbuffer);
        }

        public void UseProgram(GLProgramHandle program)
        {
            MacGL.UseProgram((int) program);
        }

        public void BindAttribLocation(GLProgramHandle program, int index, string name)
        {
            MacGL.BindAttribLocation((int) program, index, name);
        }

        public int GetAttribLocation(GLProgramHandle program, string name)
        {
            return MacGL.GetAttribLocation((int) program, name);
        }

        public int GetUniformLocation(GLProgramHandle program, string name)
        {
            return MacGL.GetUniformLocation((int) program, name);
        }

        public void Uniform1(int location, int value)
        {
            MacGL.Uniform1(location, value);
        }

        public void Uniform2(int location, Int2 value)
        {
            MacGL.Uniform2(location, value.X, value.Y);
        }

        public void Uniform3(int location, Int3 value)
        {
            MacGL.Uniform3(location, value.X, value.Y, value.Z);
        }

        public void Uniform4(int location, Int4 value)
        {
            MacGL.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }

        public void Uniform1(int location, float value)
        {
            MacGL.Uniform1(location, value);
        }

        public void Uniform2(int location, Float2 value)
        {
            MacGL.Uniform2(location, value.X, value.Y);
        }

        public void Uniform3(int location, Float3 value)
        {
            MacGL.Uniform3(location, value.X, value.Y, value.Z);
        }

        public void Uniform4(int location, Float4 value)
        {
            MacGL.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }

        public void UniformMatrix2(int location, bool transpose, Float2x2 value)
        {
            MacGL.UniformMatrix2(location, 1, transpose, ref value.M11);
        }

        public void UniformMatrix3(int location, bool transpose, Float3x3 value)
        {
            MacGL.UniformMatrix3(location, 1, transpose, ref value.M11);
        }

        public void UniformMatrix4(int location, bool transpose, Float4x4 value)
        {
            MacGL.UniformMatrix4(location, 1, transpose, ref value.M11);
        }

        public void Uniform1(int location, int[] value)
        {
            MacGL.Uniform1(location, value.Length, value);
        }

        public void Uniform2(int location, Int2[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (int* p = &value[0].X)
                        MacGL.Uniform2(location, value.Length, p);
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
                        MacGL.Uniform3(location, value.Length, p);
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
                        MacGL.Uniform4(location, value.Length, p);
                }
            }
        }

        public void Uniform1(int location, float[] value)
        {
            MacGL.Uniform1(location, value.Length, value);
        }

        public void Uniform2(int location, Float2[] value)
        {
            if (value.Length > 0)
            {
                unsafe
                {
                    fixed (float* p = &value[0].X)
                        MacGL.Uniform2(location, value.Length, p);
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
                        MacGL.Uniform3(location, value.Length, p);
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
                        MacGL.Uniform4(location, value.Length, p);
                }
            }
        }

        public void UniformMatrix2(int location, bool transpose, Float2x2[] value)
        {
            if (value.Length > 0)
                MacGL.UniformMatrix2(location, value.Length, transpose, ref value[0].M11);
        }

        public void UniformMatrix3(int location, bool transpose, Float3x3[] value)
        {
            if (value.Length > 0)
                MacGL.UniformMatrix3(location, value.Length, transpose, ref value[0].M11);
        }

        public void UniformMatrix4(int location, bool transpose, Float4x4[] value)
        {
            if (value.Length > 0)
                MacGL.UniformMatrix4(location, value.Length, transpose, ref value[0].M11);
        }

        public void EnableVertexAttribArray(int index)
        {
            MacGL.EnableVertexAttribArray(index);
        }

        public void DisableVertexAttribArray(int index)
        {
            MacGL.DisableVertexAttribArray(index);
        }

        public void VertexAttribPointer(int index, int size, GLDataType type, bool normalized, int stride, int offset)
        {
            MacGL.VertexAttribPointer(index, size, (VertexAttribPointerType)type, normalized, stride, offset);
        }

        public void DrawArrays(GLPrimitiveType mode, int first, int count)
        {
            MacGL.DrawArrays((BeginMode)mode, first, count);
        }

        public void DrawElements(GLPrimitiveType mode, int count, GLIndexType type, int offset)
        {
            MacGL.DrawElements((BeginMode)mode, count, (DrawElementsType)type, offset);
        }

        public GLBufferHandle CreateBuffer()
        {
            int r;
            MacGL.GenBuffers(1, out r);
            AddContextObject(new BufferDisposable (r));
            return new GLBufferHandle(r);
        }

        public void DeleteBuffer(GLBufferHandle buffer)
        {
            DisposeAndRemoveObject(_buffers, (int) buffer);
        }

        public void BindBuffer(GLBufferTarget target, GLBufferHandle buffer)
        {
            MacGL.BindBuffer((BufferTarget)target, (int) buffer);
        }

        public void BufferData(GLBufferTarget target, int sizeInBytes, IntPtr data, GLBufferUsage usage)
        {
            MacGL.BufferData((BufferTarget)target, (IntPtr)sizeInBytes, data, (BufferUsageHint)usage);
        }

        public void BufferSubData(GLBufferTarget target, int offset, int sizeInBytes, IntPtr data)
        {
            MacGL.BufferSubData((BufferTarget)target, (IntPtr)offset, (IntPtr)sizeInBytes, data);
        }

        public void Enable(GLEnableCap cap)
        {
            MacGL.Enable((EnableCap)cap);
        }

        public void Disable(GLEnableCap cap)
        {
            MacGL.Disable((EnableCap)cap);
        }

        public bool IsEnabled(GLEnableCap cap)
        {
            return MacGL.IsEnabled((EnableCap)cap);
        }

        public void BlendFunc(GLBlendingFactor src, GLBlendingFactor dst)
        {
            MacGL.BlendFunc((BlendingFactorSrc)src, (BlendingFactorDest)dst);
        }

        public void BlendFuncSeparate(GLBlendingFactor srcRGB, GLBlendingFactor dstRGB, GLBlendingFactor srcAlpha, GLBlendingFactor dstAlpha)
        {
            MacGL.BlendFuncSeparate((BlendingFactorSrc)srcRGB, (BlendingFactorDest)dstRGB, 
                (BlendingFactorSrc)srcAlpha, (BlendingFactorDest)dstAlpha);
        }

        public void BlendEquation(GLBlendEquation mode)
        {
            MacGL.BlendEquation((BlendEquationMode)mode);
        }

        public void BlendEquationSeparate(GLBlendEquation modeRgb, GLBlendEquation modeAlpha)
        {
            MacGL.BlendEquationSeparate((BlendEquationMode)modeRgb, (BlendEquationMode)modeAlpha);
        }

        public void CullFace(GLCullFaceMode mode)
        {
            MacGL.CullFace((CullFaceMode)mode);
        }

        public void FrontFace(GLFrontFaceDirection mode)
        {
            MacGL.FrontFace((FrontFaceDirection)mode);
        }

        public void DepthFunc(GLDepthFunction func)
        {
            MacGL.DepthFunc((DepthFunction)func);
        }

        public void Scissor(int x, int y, int width, int height)
        {
            MacGL.Scissor(x, y, width, height);
        }

        public void Viewport(int x, int y, int width, int height)
        {
            MacGL.Viewport(x, y, width, height);
        }

        public void LineWidth(float width)
        {
            MacGL.LineWidth(width);
        }

        public void PolygonOffset(float factor, float units)
        {
            MacGL.PolygonOffset(factor, units);
        }

        public void DepthRange(float zNear, float zFar)
        {
            MacGL.DepthRange(zNear, zFar);
        }

        public GLShaderHandle CreateShader(GLShaderType type)
        {
            var shader = MacGL.CreateShader((ShaderType)type);
            AddContextObject(new ShaderDisposable (shader));
            return new GLShaderHandle(shader);
        }

        public void DeleteShader(GLShaderHandle shader)
        {
            DisposeAndRemoveObject(_shaders, (int) shader);
        }

        public void ShaderSource(GLShaderHandle shader, string source)
        {
            MacGL.ShaderSource((int) shader, source);
        }

        public void ReadPixels(int x, int y, int width, int height, GLPixelFormat format, GLPixelType type, byte[] buffer)
        {
            MacGL.ReadPixels(x, y, width, height, (PixelFormat)format, (PixelType)type, buffer);
        }

        public void CompileShader(GLShaderHandle shader)
        {
            MacGL.CompileShader((int) shader);
        }

        public int GetShaderParameter(GLShaderHandle shader, GLShaderParameter pname)
        {
            int result;
            MacGL.GetShader((uint) (int) shader, (ShaderParameter)pname, out result);
            return result;
        }

        public string GetShaderInfoLog(GLShaderHandle shader)
        {
            return MacGL.GetShaderInfoLog((int) shader);
        }

        public GLProgramHandle CreateProgram()
        {
            var program = MacGL.CreateProgram();
            AddContextObject(new ProgramDisposable (program));
            return new GLProgramHandle(program);
        }

        public void DeleteProgram(GLProgramHandle program)
        {
            DisposeAndRemoveObject(_programs, (int) program);
        }

        public void AttachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            MacGL.AttachShader((int) program, (int) shader);
        }

        public void DetachShader(GLProgramHandle program, GLShaderHandle shader)
        {
            MacGL.DetachShader((int) program, (int) shader);
        }

        public void LinkProgram(GLProgramHandle program)
        {
            MacGL.LinkProgram((int) program);
        }

        public int GetProgramParameter(GLProgramHandle program, GLProgramParameter pname)
        {
            int result;
            MacGL.GetProgram((uint) (int) program, (ProgramParameter)pname, out result);
            return result;
        }

        public string GetProgramInfoLog(GLProgramHandle program)
        {
            return MacGL.GetShaderInfoLog((int) program);
        }

        public bool HasGetShaderPrecisionFormat
        {
            get { return false; }
        }

        public GLShaderPrecisionFormat GetShaderPrecisionFormat(GLShaderType shaderType, GLShaderPrecision precision)
        {
            return new GLShaderPrecisionFormat();
        }

        public string GetString(GLStringName name)
        {
            return MacGL.GetString((StringName)name);
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
