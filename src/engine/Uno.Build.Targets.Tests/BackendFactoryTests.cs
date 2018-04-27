using NUnit.Framework;
using Uno.Build.Targets.PInvoke;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CIL;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.Compiler.Backends.CSharp;
using Uno.Compiler.Backends.JavaScript;
using Uno.Compiler.Backends.OpenGL;
using Uno.Compiler.Backends.PInvoke;
using Uno.Compiler.Extensions;

namespace Uno.Build.Targets.Tests
{
    /// <summary>
    /// Tests for checking that dynamic loading of backend assemblies works correctly
    /// </summary>
    [TestFixture]
    public class BackendFactoryTests
    {
        [Test]
        public void NewGLBackend_returns_backend()
        {
            Assert.IsInstanceOf<GLBackend>(BackendFactory.NewGLBackend());
        }

        [Test]
        public void NewCppBackend_returns_backend()
        {
            Assert.IsInstanceOf<CppBackend>(BackendFactory.NewCppBackend(ShaderBackend.Dummy, new CppExtension()));
        }

        [Test]
        public void NewJsBackend_returns_backend()
        {
            Assert.IsInstanceOf<JsBackend>(BackendFactory.NewJsBackend(ShaderBackend.Dummy));
        }

        [Test]
        public void NewPInvokeBackend_returns_backend()
        {
            Assert.IsInstanceOf<PInvokeBackend>(BackendFactory.NewPInvokeBackend(ShaderBackend.Dummy));
        }

        [Test]
        public void NewCsBackend_returns_backend()
        {
            Assert.IsInstanceOf<CsBackend>(BackendFactory.NewCsBackend());
        }

        [Test]
        public void NewCilBackend_returns_backend()
        {
            Assert.IsInstanceOf<CilBackend>(BackendFactory.NewCilBackend(ShaderBackend.Dummy));
        }
    }
}