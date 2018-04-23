namespace Uno.Compiler.Backends.CIL
{
    struct CilMember<TBuilder, TDefinition>
    {
        public readonly TBuilder Builder;
        public readonly TDefinition Definition;

        public CilMember(TBuilder builder, TDefinition definition)
        {
            Builder = builder;
            Definition = definition;
        }
    }
}