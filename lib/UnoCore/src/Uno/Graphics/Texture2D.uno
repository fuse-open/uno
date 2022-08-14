using OpenGL;
using Uno.Graphics.OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Diagnostics;
using Uno.IO;

namespace Uno.Graphics
{
    public sealed intrinsic class Texture2D : IDisposable
    {
        public int2 Size
        {
            get;
            private set;
        }

        static int _maxSize = 0;
        public static int MaxSize
        {
            get {
                if (_maxSize == 0)
                {
                    if defined(OPENGL)
                        _maxSize = GL.GetInteger(GLIntegerName.MaxTextureSize);
                    else
                        build_error;
                }

                return _maxSize;
            }
        }

        extern(OPENGL) static bool CheckGLES3Support()
        {
            var versionString = GL.GetString(GLStringName.Version);
            if (versionString.StartsWith("OpenGL ES "))
            {
                var majorVersionString = versionString.Substring(10).Split(new char[] { '.' })[0];
                try
                {
                    return int.Parse(majorVersionString) >= 3;
                }
                catch (FormatException e)
                {
                    debug_log "**** Invalid version string: " + versionString;
                }
            }

            return false;
        }

        extern(OPENGL) static bool CheckExtensionSupport(string extensionName)
        {
            var extensions = GL.GetString(GLStringName.Extensions).Split(new char[] { ' ' });
            foreach (var e in extensions)
            {
                if (e.Equals(extensionName))
                    return true;
            }
            return false;
        }

        static bool _haveNonPow2SupportValid;
        static bool _haveNonPow2Support;
        public static bool HaveNonPow2Support
        {
            get
            {
                if (!_haveNonPow2SupportValid)
                {
                    if defined(OPENGL)
                        _haveNonPow2Support = CheckGLES3Support() || CheckExtensionSupport("GL_OES_texture_npot");
                    else
                        build_error;

                    _haveNonPow2SupportValid = true;
                }

                return _haveNonPow2Support;
            }
        }

        public int MipCount
        {
            get;
            private set;
        }

        public Format Format
        {
            get;
            private set;
        }

        public extern(OPENGL) GLTextureHandle GLTextureHandle
        {
            get;
            private set;
        }

        public extern(OPENGL) Texture2D(GLTextureHandle handle, int2 size, int mipCount, Format format)
        {
            GLTextureHandle = handle;
            Size = size;
            MipCount = mipCount;
            Format = format;
        }

        public Texture2D(int2 size, Format format, bool mipmap)
        {
            if defined(OPENGL)
                GLTextureHandle = GL.CreateTexture();
            else
                build_error;

            Size = size;
            Format = format;
            MipCount = mipmap ? TextureHelpers.GetMipCount(size) : 1;

            Update((byte[])null);
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Texture2D");
            else if defined(OPENGL)
                GL.DeleteTexture(GLTextureHandle);
            else
                build_error;

            IsDisposed = true;
        }

        public bool CanUpdate
        {
            get { return Format != Format.Unknown; }
        }

        public void Update(IntPtr mip0)
        {
            if (Format == Format.Unknown)
            {
                throw new InvalidOperationException("Texture is immutable and cannot be updated");
            }
            else if defined(OPENGL)
            {
                GL.ActiveTexture(GLTextureUnit.Texture0);
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.MagFilter, GLTextureParameterValue.Linear);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.MinFilter, GLTextureParameterValue.Linear);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.WrapS, GLTextureParameterValue.ClampToEdge);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.WrapT, GLTextureParameterValue.ClampToEdge);
                GLHelper.TexImage2DFromIntPtr(GLTextureTarget.Texture2D, Size.X, Size.Y, 0, Format, mip0);
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle.Zero);
            }
            else
            {
                build_error;
            }
        }

        public void Update(byte[] mip0)
        {
            if (Format == Format.Unknown)
            {
                throw new InvalidOperationException("Texture is immutable and cannot be updated");
            }
            else if defined(OPENGL)
            {
                GL.ActiveTexture(GLTextureUnit.Texture0);
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.MagFilter, GLTextureParameterValue.Linear);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.MinFilter, GLTextureParameterValue.Linear);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.WrapS, GLTextureParameterValue.ClampToEdge);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.WrapT, GLTextureParameterValue.ClampToEdge);
                GLHelper.TexImage2DFromBytes(GLTextureTarget.Texture2D, Size.X, Size.Y, 0, Format, mip0);
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle.Zero);
            }
            else
            {
                build_error;
            }
        }

        public void Update(int firstMip, params byte[][] mips)
        {
            if (Format == Format.Unknown)
            {
                throw new InvalidOperationException("Texture is immutable and cannot be updated");
            }
            else if defined(OPENGL)
            {
                GL.ActiveTexture(GLTextureUnit.Texture0);
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.MagFilter, GLTextureParameterValue.Linear);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.MinFilter, GLTextureParameterValue.Linear);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.WrapS, GLTextureParameterValue.ClampToEdge);
                GL.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.WrapT, GLTextureParameterValue.ClampToEdge);

                int w = Size.X;
                int h = Size.Y;

                for (int i = 0; i < MipCount; i++)
                {
                    if (i >= firstMip)
                        GLHelper.TexImage2DFromBytes(GLTextureTarget.Texture2D, w, h, i, Format, mips[i]);

                    w = w >> 1;
                    h = h >> 1;

                    if (w < 1)
                        w = 1;

                    if (h < 1)
                        h = 1;

                    if (i >= mips.Length - firstMip)
                        break;
                }

                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle.Zero);
            }
            else
            {
                build_error;
            }
        }

        public bool IsPow2
        {
            get { return Math.IsPow2(Size.X) && Math.IsPow2(Size.Y); }
        }

        public bool IsMipmap
        {
            get { return MipCount > 1 && IsPow2; }
        }

        public void GenerateMipmap()
        {
            if (!IsMipmap)
            {
                throw new InvalidOperationException("Texture does not support mipmap");
            }
            else if defined(OPENGL)
            {
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle);
                GL.GenerateMipmap(GLTextureTarget.Texture2D);
                GL.BindTexture(GLTextureTarget.Texture2D, GLTextureHandle.Zero);
            }
            else
            {
                build_error;
            }
        }
    }
}
