using System.Text;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.JavaScript
{
    class JsDecompiler : Decompiler
    {
        readonly JsBackend Backend;

        public JsDecompiler(JsBackend backend)
            : base(false)
        {
            Backend = backend;
        }

        public override SourceWriter CreateWriter(Source src, StringBuilder sb, Function context)
        {
            return new JsWriter(Backend, sb, context);
        }

        public override string GetEntity(Source src, Function context, IEntity entity)
        {
            string result;
            return Backend.Globals.TryGetValue(entity, out result) ||
                   Backend.Members.TryGetValue(entity, out result)
                    ? result
                    : entity.ToString();
        }
    }
}
