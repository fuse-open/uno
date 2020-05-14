using System;

namespace Uno.Compiler.API.Domain.Extensions
{
    public struct CopyFile
    {
        public readonly CopyFileFlags Flags;
        public readonly SourceValue SourceName;
        public readonly SourceValue? TargetName;
        public readonly SourceValue? Condition;
        public readonly SourceValue? Type;
        public readonly Func<string, string> Preprocess;

        public CopyFile(SourceValue sourceName, CopyFileFlags flags, SourceValue? targetName = null, SourceValue? cond = null, SourceValue? type = null, Func<string, string> preprocess = null)
        {
            Flags = flags;
            SourceName = sourceName;
            TargetName = targetName;
            Condition = cond;
            Type = type;
            Preprocess = preprocess ?? (x => x);

            if (string.IsNullOrEmpty(SourceName.String))
                throw new ArgumentNullException(nameof(sourceName));
        }

        public override string ToString()
        {
            return SourceName.String;
        }
    }
}