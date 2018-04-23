using System.Collections.Generic;
using System.IO;

namespace Uno.CompilerTestRunner
{
    class ErrorItemComparer : IEqualityComparer<ErrorItem>
    {
        public bool Equals(ErrorItem left, ErrorItem right)
        {
            return left.ErrorCode == right.ErrorCode &&
                (string.IsNullOrEmpty(left.Message) || string.IsNullOrEmpty(right.Message) || left.Message.Equals(right.Message)) &&
                (left.Source == null || right.Source == null || (left.Source.Line == right.Source.Line && PathIsSame(left.Source.FullPath, right.Source.FullPath)));
        }

        private static bool PathIsSame(string left, string right)
        {
            if (Path.IsPathRooted(left))
                return left.EndsWith(right);
            if (Path.IsPathRooted(right))
                return left.EndsWith(left);
            return left.Equals(right);
        }

        public int GetHashCode(ErrorItem obj)
        {
            return obj.ErrorCode.GetHashCode();
        }

    }
}
