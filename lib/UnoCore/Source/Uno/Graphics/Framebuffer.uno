namespace Uno.Graphics
{
    [Flags]
    public enum FramebufferFlags
    {
        None = 0,
        DepthBuffer = 1 << 0,
        Mipmap = 1 << 1,
    }

    public sealed class Framebuffer : IDisposable
    {
        public Texture2D ColorBuffer
        {
            get;
            private set;
        }

        public RenderTarget RenderTarget
        {
            get;
            private set;
        }

        public int2 Size
        {
            get { return RenderTarget.Size; }
        }

        public bool HasDepth
        {
            get { return RenderTarget.HasDepth; }
        }

        public Format Format
        {
            get { return ColorBuffer.Format; }
        }

        public Framebuffer(int2 size, Format format, FramebufferFlags flags)
        {
            ColorBuffer = new texture2D(size, format, flags.HasFlag(FramebufferFlags.Mipmap));
            RenderTarget = RenderTarget.Create(ColorBuffer, 0, flags.HasFlag(FramebufferFlags.DepthBuffer));
        }

        public void Dispose()
        {
            ColorBuffer.Dispose();
            RenderTarget.Dispose();
        }

        public void GenerateMipmap()
        {
            ColorBuffer.GenerateMipmap();
        }
    }
}
