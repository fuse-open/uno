namespace Uno.Compiler.Backends.UnoDoc
{
    public class GeneratorSettings
    {
        public int CommentScannerMaxOffset { get; }

        public GeneratorSettings(int commentScannerMaxOffset)
        {
            CommentScannerMaxOffset = commentScannerMaxOffset;
        }
    }
}
