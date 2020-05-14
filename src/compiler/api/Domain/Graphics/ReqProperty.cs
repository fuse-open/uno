using System.Text;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class ReqProperty : ReqStatement
    {
        public readonly string PropertyName;
        public readonly DataType PropertyType;
        public readonly uint Offset;
        public readonly string Tag;

        public override ReqStatementType Type => ReqStatementType.Property;

        public ReqProperty(Source src, string name, DataType type, uint offset, string tag)
            : base(src)
        {
            PropertyName = name;
            PropertyType = type;
            Offset = offset;
            Tag = tag;
        }

        public override string ToString()
        {
            var s = new StringBuilder("req(");

            if (Offset > 0)
                s.Append("prev(" + Offset + ") ");

            s.Append(PropertyName);

            if (PropertyType != null)
                s.Append(" as " + PropertyType);

            if (Tag != null)
                s.Append(" tag " + Tag.ToLiteral());

            s.Append(")");
            return s.ToString();
        }
    }
}