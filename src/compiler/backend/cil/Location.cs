using System;

namespace Uno.Compiler.Backends.CIL
{
    public class Location: IComparable<Location>
    {
        public int ILOffset { get; }
        public string Path { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public Location(int ilOffset, string path, int line, int column)
        {
            ILOffset = ilOffset;
            Path = path;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Location other)
        {
            return other.ILOffset - ILOffset;
        }
    }
}