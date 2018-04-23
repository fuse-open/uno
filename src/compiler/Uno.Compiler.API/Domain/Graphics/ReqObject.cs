using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class ReqObject : ReqStatement
    {
        public readonly DataType ObjectType;

        public override ReqStatementType Type => ReqStatementType.Object;

        public ReqObject(Source src, DataType type)
            : base(src)
        {
            ObjectType = type;
        }

        public override string ToString()
        {
            return "req(this)";
        }
    }
}