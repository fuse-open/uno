using System;
using System.Text;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.CIL
{
    public class PInvokeDecompiler : Decompiler
    {
        readonly PInvokeBackend _backend;
        public PInvokeDecompiler(PInvokeBackend backend)
        {
            _backend = backend;
        }

        public override string GetType(Source src, Function context, DataType dt)
        {
            return _backend.GetForeignParamType(src, dt);
        }

        public override SourceWriter CreateWriter(Source src, StringBuilder sb, Function context)
        {
            return new PInvokeWriter(_backend, sb, context);
        }
    }
}