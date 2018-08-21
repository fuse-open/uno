namespace Uno.Graphics
{
    // TODO/Note: GL values
    public enum TextureFilter
    {
        Nearest = 0x2600,
        Linear = 0x2601,
        NearestMipmapNearest = 0x2700,
        LinearMipmapNearest = 0x2701,
        NearestMipmapLinear = 0x2702,
        LinearMipmapLinear = 0x2703
    }

    // TODO/Note: GL values
    public enum TextureAddressMode
    {
        Wrap = 0x2901,
        Clamp = 0x812F,
    }

    public struct SamplerState
    {
        public TextureFilter MinFilter;
        public TextureFilter MagFilter;
        public TextureAddressMode AddressU;
        public TextureAddressMode AddressV;
        public TextureAddressMode AddressW;

        public TextureFilter MinFilterNoMipmap
        {
            get
            {
                switch (MinFilter)
                {
                    case TextureFilter.NearestMipmapNearest:
                    case TextureFilter.LinearMipmapNearest:
                        return TextureFilter.Nearest;

                    case TextureFilter.NearestMipmapLinear:
                    case TextureFilter.LinearMipmapLinear:
                        return TextureFilter.Linear;

                    default:
                        return MinFilter;
                }
            }
        }

        public SamplerState(TextureFilter minFilter, TextureFilter magFilter, TextureAddressMode addressMode)
        {
            MinFilter = minFilter;
            MagFilter = magFilter;
            AddressU = addressMode;
            AddressV = addressMode;
            AddressW = addressMode;
        }

        public SamplerState(TextureFilter minFilter, TextureFilter magFilter, TextureAddressMode addressU, TextureAddressMode addressV, TextureAddressMode addressW)
        {
            MinFilter = minFilter;
            MagFilter = magFilter;
            AddressU = addressU;
            AddressV = addressV;
            AddressW = addressW;
        }

        public static SamplerState NearestWrap
        {
            get { return new SamplerState(TextureFilter.Nearest, TextureFilter.Nearest, TextureAddressMode.Wrap); }
        }

        public static SamplerState NearestClamp
        {
            get { return new SamplerState(TextureFilter.Nearest, TextureFilter.Nearest, TextureAddressMode.Clamp); }
        }

        public static SamplerState LinearWrap
        {
            get { return new SamplerState(TextureFilter.Linear, TextureFilter.Linear, TextureAddressMode.Wrap); }
        }

        public static SamplerState LinearClamp
        {
            get { return new SamplerState(TextureFilter.Linear, TextureFilter.Linear, TextureAddressMode.Clamp); }
        }

        public static SamplerState TrilinearWrap
        {
            get { return new SamplerState(TextureFilter.LinearMipmapLinear, TextureFilter.Linear, TextureAddressMode.Wrap); }
        }

        public static SamplerState TrilinearClamp
        {
            get { return new SamplerState(TextureFilter.LinearMipmapLinear, TextureFilter.Linear, TextureAddressMode.Clamp); }
        }
    }
}
