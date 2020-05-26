using OpenGL;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Graphics.OpenGL
{
    public extern(OPENGL) sealed class GLCompiledProgram : IDisposable
    {
        readonly GLShaderHandle _vsHandle;
        readonly GLShaderHandle _fsHandle;
        readonly int[] _locations;

        public GLProgramHandle GLProgramHandle
        {
            get;
            private set;
        }

        public int LocationCount
        {
            get { return _locations.Length; }
        }

        public int GetLocation(int index)
        {
            return _locations[index];
        }

        internal GLCompiledProgram(string vsSource, string fsSource, int constCount, int attribCount, string[] constAttribAndUniformNames)
        {
            _vsHandle = GLHelper.CompileShader(GLShaderType.VertexShader, vsSource);
            _fsHandle = GLHelper.CompileShader(GLShaderType.FragmentShader, fsSource);

            GLProgramHandle = GLHelper.LinkProgram(_vsHandle, _fsHandle);

            _locations = new int[constAttribAndUniformNames.Length];

            for (int i = 0; i < constCount; i++)
                _locations[i] = -1;

            for (int i = constCount; i < constCount + attribCount; i++)
                _locations[i] = GL.GetAttribLocation(GLProgramHandle, constAttribAndUniformNames[i]);

            for (int i = constCount + attribCount; i < constAttribAndUniformNames.Length; i++)
                _locations[i] = GL.GetUniformLocation(GLProgramHandle, constAttribAndUniformNames[i]);
        }

        public void Dispose()
        {
            GL.UseProgram(GLProgramHandle.Zero);
            GL.DetachShader(GLProgramHandle, _vsHandle);
            GL.DetachShader(GLProgramHandle, _fsHandle);
            GL.DeleteProgram(GLProgramHandle);
            GL.DeleteShader(_vsHandle);
            GL.DeleteShader(_fsHandle);
        }
    }
}
