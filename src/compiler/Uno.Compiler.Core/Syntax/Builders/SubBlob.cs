namespace Uno.Compiler.Core.Syntax.Builders
{
    struct SubBlob
    {
        public readonly int Offset;
        public readonly byte[] Data;

        public SubBlob(int offset, byte[] data)
        {
            Offset = offset;
            Data = data;
        }
    }
}