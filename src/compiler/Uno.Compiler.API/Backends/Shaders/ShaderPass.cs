using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Backends.Shaders
{
    public abstract class ShaderPass : Pass
    {
        protected abstract Statement TransformDraw(Draw draw);

        protected ShaderPass(ShaderBackend backend)
            : base(backend)
        {
        }

        public override bool Begin(Namespace ns)
        {
            ns.Blocks.Clear();
            return true;
        }

        public override bool Begin(DataType dt)
        {
            dt.SetBlock(null);
            return true;
        }

        public override bool Begin(Function f)
        {
            var method = f as Method;

            if (method == null ||
                method.DrawBlocks.Count == 0 ||
                !method.HasBody)
                return false;

            method.DrawBlocks.Clear();
            return true;
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.Draw:
                    e = TransformDraw((Draw) e);
                    break;
            }
        }
    }
}
