using Uno.Graphics;

namespace Uno.Content.Fonts
{
    [Obsolete]
    public abstract class TextTransform
    {
        public abstract float4x4 Matrix
        {
            get;
            set;
        }

        public abstract float4x4 ResolveClipSpaceMatrix();

        public virtual PolygonFace CullFace { get { return PolygonFace.None; } }
    }
}
