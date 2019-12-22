namespace Uno.Graphics.Utils.Text
{
    public abstract class TextTransform
    {
        public float4x4 Matrix { get; set; }

        public TextTransform()
        {
            Matrix = float4x4.Identity;
        }

        public abstract float4x4 ResolveClipSpaceMatrix();

        public virtual PolygonFace CullFace
        {
            get { return PolygonFace.None; }
        }
    }
}
