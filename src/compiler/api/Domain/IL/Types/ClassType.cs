namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class ClassType : GenericType
    {
        public override TypeType TypeType => TypeType.Class;

        public ClassType(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, comment, modifiers, name)
        {
        }

        protected override GenericType CreateParameterizable()
        {
            return new ClassType(Source, Parent, DocComment, Modifiers, UnoName);
        }
    }
}
