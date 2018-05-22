using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;

namespace Uno.Runtime.Implementation.ShaderBackends.OpenGL
{
    public extern(OPENGL) sealed class GLProgram
    {
        Dictionary<string, GLCompiledProgram> _cachedPrograms;
        GLCompiledProgram _singleProgram;

        readonly string _vsSource;
        readonly string _fsSource;
        readonly string[] _constAttribAndUniformNames;

        readonly int _constCount;
        readonly int _attribCount;

        public int ConstantCount
        {
            get { return _constCount; }
        }

        internal GLProgram(string vsSource, string fsSource, int constCount, int attribCount, string[] constAttribAndUniformNames)
        {
            _vsSource = vsSource;
            _fsSource = fsSource;
            _constCount = constCount;
            _attribCount = attribCount;
            _constAttribAndUniformNames = constAttribAndUniformNames;
        }

        public static GLProgram Create(string vsSource, string fsSource, int constCount, int attribCount, params string[] constAttribAndUniformNames)
        {
            return new GLProgram(vsSource, fsSource, constCount, attribCount, constAttribAndUniformNames);
        }

        GLCompiledProgram GetCompiledProgramInternal(string[] constStrings)
        {
            var vsPrefix = "#ifdef GL_ES\nprecision highp float;\n#endif\n";
            var fsPrefix = "#ifdef GL_ES\n#extension GL_OES_standard_derivatives : enable\n";

            if defined(ANDROID)
                fsPrefix += "#extension GL_OES_EGL_image_external : enable\n";  // extension directive must occur before precision specifier

            fsPrefix += "# ifdef GL_FRAGMENT_PRECISION_HIGH\nprecision highp float;\n# else\nprecision mediump float;\n# endif\n#endif\n";

            // TODO: StringBuilder
            var defines = "";

            for (int i = 0; i < constStrings.Length; i++)
                defines += "#define " + _constAttribAndUniformNames[i] + " " + constStrings[i] + "\n";

            return new GLCompiledProgram(vsPrefix + defines + _vsSource,
                                         fsPrefix + defines + _fsSource,
                                         _constCount,
                                         _attribCount,
                                         _constAttribAndUniformNames);
        }

        public GLCompiledProgram GetCompiledProgram(params string[] constStrings)
        {
            if (constStrings.Length == 0)
            {
                if (_singleProgram == null)
                    _singleProgram = GetCompiledProgramInternal(constStrings);

                return _singleProgram;
            }

            var key = string.Join(":", constStrings);

            if (_cachedPrograms == null)
                _cachedPrograms = new Dictionary<string, GLCompiledProgram>();

            GLCompiledProgram result;
            if (!_cachedPrograms.TryGetValue(key, out result))
            {
                //debug_log "Compiling shader with " + constStrings.Length + " runtime constants '" + key + "'";
                result = GetCompiledProgramInternal(constStrings);
                _cachedPrograms.Add(key, result);
            }

            return result;
        }
    }
}
