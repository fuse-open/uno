namespace Uno.Compiler.API.Domain.Graphics
{
    public abstract class BlockMember : SourceObject
    {
        protected BlockMember(Source src)
            : base(src)
        {
        }

        public abstract BlockMemberType Type
        {
            get;
        }
    }
}