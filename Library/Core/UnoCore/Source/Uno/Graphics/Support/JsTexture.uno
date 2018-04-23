using OpenGL;

namespace Uno.Graphics.Support
{
    extern(JAVASCRIPT)
    static class JsTexture
    {
        public static texture2D Load2D(string filename)
        @{
            var handle = gl.createTexture(),
                target = @{GLTextureTarget.Texture2D},
                format = @{GLPixelFormat.Rgba},
                image = $BundleImages[$0],
                w = image.width,
                h = image.height,
                mc = 1,
                size = @{int2(int,int):New(w, h)};

            gl.bindTexture(target, handle);
            gl.texImage2D(target, 0, format, format, @{GLPixelType.UnsignedByte}, image);

            if ((w == (w & -w)) && (h == (h & -h))) {
                gl.generateMipmap(target);
                while (w > 1 || h > 1) {
                    w >>= 1;
                    h >>= 1;
                    mc++;
                }
            }

            gl.bindTexture(target, null);
            return @{Texture2D(GLTextureHandle,int2,int,Format):New(handle, size, mc, @{Format.RGBA8888})};
        @}

        public static textureCube LoadCube(string filename)
        @{
            // TODO: doesn't work in Safari
            var handle = gl.createTexture(),
                target = @{GLTextureTarget.TextureCubeMap},
                format = @{GLPixelFormat.Rgba},
                type = @{GLPixelType.UnsignedByte},
                face = @{GLTextureTarget.TextureCubeMapPositiveX},
                image = $BundleImages[$0],
                size = image.width >> 1,
                mc = 1,
                u = [0,1,0,1,0,1],
                v = [0,0,1,1,2,2],
                d = document.createElement('canvas'),
                c = d.getContext('2d');

            d.width = image.width;
            d.height = image.height;
            c.drawImage(image, 0, 0);

            gl.bindTexture(target, handle);

            for (var i = 0; i < 6; i++)
                gl.texImage2D(face + i, 0, format, format, type, c.getImageData(u[i] * size, v[i] * size, size, size));

            if (size == (size & -size)) {
                gl.generateMipmap(target);
                while (size > 1) {
                    size >>= 1;
                    mc++;
                }
            }

            gl.bindTexture(target, null);
            var format = @{Format.RGBA8888};
            return @{TextureCube(GLTextureHandle,int,int,Format):New(handle, size, mc, format)};
        @}
    }
}
