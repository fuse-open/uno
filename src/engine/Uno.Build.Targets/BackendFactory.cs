using System;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Extensions;

namespace Uno.Build.Targets
{
    static class BackendFactory
    {
        public static ShaderBackend NewGLBackend()
        {
            return CreateInstance<ShaderBackend>("Uno.Compiler.Backends.OpenGL", "Uno.Compiler.Backends.OpenGL.GLBackend");
        }

        public static Backend NewUnoDocBackend()
        {
            return CreateInstance<Backend>("Uno.Compiler.Backends.UnoDoc", "Uno.Compiler.Backends.UnoDoc.UnoDocBackend");
        }

        public static Backend NewCppBackend(ShaderBackend shaderBackend, CppExtension cppExtension)
        {
            return CreateInstance<Backend>("Uno.Compiler.Backends.CPlusPlus", "Uno.Compiler.Backends.CPlusPlus.CppBackend", shaderBackend, cppExtension);
        }

        public static Backend NewJsBackend(ShaderBackend shaderBackend)
        {
            return CreateInstance<Backend>("Uno.Compiler.Backends.JavaScript", "Uno.Compiler.Backends.JavaScript.JsBackend", shaderBackend);
        }

        public static Backend NewPInvokeBackend(ShaderBackend shaderBackend)
        {
            return CreateInstance<Backend>("Uno.Compiler.Backends.PInvoke", "Uno.Compiler.Backends.PInvoke.PInvokeBackend", shaderBackend);
        }

        public static Backend NewCsBackend()
        {
            return CreateInstance<Backend>("Uno.Compiler.Backends.CSharp", "Uno.Compiler.Backends.CSharp.CsBackend");
        }

        public static Backend NewCilBackend(ShaderBackend shaderBackend)
        {
            return CreateInstance<Backend>("Uno.Compiler.Backends.CIL", "Uno.Compiler.Backends.CIL.CilBackend", shaderBackend);
        }

        private static T CreateInstance<T>(string assemblyName, string typeName, params object[] args)
        {
            return (T)Activator.CreateInstance(assemblyName, typeName, false, 0, null, args, null, null).Unwrap();
        }
    }
}