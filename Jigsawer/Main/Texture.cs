
using Jigsawer.Resources;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Drawing;
using System.Drawing.Imaging;

namespace Jigsawer.Main;

public struct Texture {
    private static int boundId;

    public int Id { get; private set; }
    public Vector2 Size { get; private set; }
    private int Unit { get; set; }

    public void Use() {
        GL.BindTextureUnit(Unit, Id);
    }

    public void Delete() => GL.DeleteTexture(Id);

    public static void Unbind() {
        GL.BindTextureUnit(0, 0);
    }

    public static Texture Create(int unit, string name) {
        GL.CreateTextures(TextureTarget.Texture2D, 1, out int textureId);

        var texture = new Texture() {
            Id = textureId,
            Unit = unit
        };

        using var imageStream = EmbeddedResourceLoader.GetResourceStream(Images.Image.GetPath(name));

        using var bitmap = new Bitmap(imageStream);

        texture.Size = new Vector2(bitmap.Width, bitmap.Height);

        BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        GL.TextureStorage2D(textureId, 1, SizedInternalFormat.Rgb8, bitmap.Width, bitmap.Height);
        GL.TextureSubImage2D(textureId, 0, 0, 0, bitmap.Width, bitmap.Height,
            OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.UnsignedByte, bitmapData.Scan0);

        bitmap.UnlockBits(bitmapData);

        return texture;
    }

    public void SetWrapping(TextureParameterName parameter, TextureWrapMode value) {
        GL.TextureParameter(Id, parameter, (int)value);
    }

    public void SetMinFilter(TextureMinFilter filter) {
        GL.TextureParameter(Id, TextureParameterName.TextureMinFilter, (int)filter);
    }

    public void SetMagFilter(TextureMagFilter filter) {
        GL.TextureParameter(Id, TextureParameterName.TextureMinFilter, (int)filter);
    }
}
