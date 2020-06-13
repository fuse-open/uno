using System.Text;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.CIL
{
    public class PInvokeWriter : SourceWriter
    {
        readonly PInvokeBackend _backend;

        public PInvokeWriter(PInvokeBackend backend, string fileName)
            : base(backend, fileName)
        {
            _backend = backend;
        }

        public PInvokeWriter(PInvokeBackend backend, StringBuilder sb, Function context)
            : base(backend, sb, context)
        {
            _backend = backend;
        }

        public override void WriteParameter(Source src, DataType dt, ParameterModifier m, string name)
        {
            if (m == ParameterModifier.Out || m == ParameterModifier.Ref)
            {
                WriteReturnType(src, dt);
                Write('*');
            }
            else if (dt.IsArray)
            {
                WriteReturnType(src, dt.ElementType);
                Write('*');
            }
            else
            {
                WriteType(src, dt);
            }
            Write(" " + name);
        }

        public override void WriteType(Source src, DataType dt)
        {
            Write(_backend.GetForeignParamType(src, dt));
        }

        public void WriteReturnType(Source src, DataType dt)
        {
            Write(_backend.GetForeignType(src, dt));
        }
    }
}
