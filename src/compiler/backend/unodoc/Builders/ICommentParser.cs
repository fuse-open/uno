namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public interface ICommentParser
    {
        SourceComment Read(SourceObject entity);
    }
}