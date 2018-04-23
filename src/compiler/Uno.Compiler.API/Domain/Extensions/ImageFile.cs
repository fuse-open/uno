namespace Uno.Compiler.API.Domain.Extensions
{
    public struct ImageFile
    {
        public readonly SourceValue SourceName;
        public readonly SourceValue? Condition;
        public readonly SourceValue? TargetName;
        public readonly int? TargetWidth;
        public readonly int? TargetHeight;

        public ImageFile(SourceValue sourceName, SourceValue? cond, SourceValue? targetName, int? targetWidth, int? targetHeight)
        {
            SourceName = sourceName;
            Condition = cond;
            TargetName = targetName;
            TargetWidth = targetWidth;
            TargetHeight = targetHeight;
        }

        public override string ToString()
        {
            return SourceName.String;
        }
    }
}
