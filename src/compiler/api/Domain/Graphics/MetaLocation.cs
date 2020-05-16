using System;

namespace Uno.Compiler.API.Domain.Graphics
{
    public struct MetaLocation : IEquatable<MetaLocation>
    {
        public readonly int NodeIndex;
        public readonly int BlockIndex;

        public MetaLocation(int nodeIndex, int blockIndex)
        {
            NodeIndex = nodeIndex;
            BlockIndex = blockIndex;
        }

        public static bool operator ==(MetaLocation a, MetaLocation b)
        {
            return a.NodeIndex == b.NodeIndex && a.BlockIndex == b.BlockIndex;
        }

        public static bool operator !=(MetaLocation a, MetaLocation b)
        {
            return !(a == b);
        }

        public bool Equals(MetaLocation other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is MetaLocation && Equals((MetaLocation)obj);
        }

        public override int GetHashCode()
        {
            int hash = 27;
            hash = (13 * hash) + NodeIndex;
            hash = (13 * hash) + BlockIndex;
            return hash;
        }
    }
}