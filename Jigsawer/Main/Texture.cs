
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
    private TextureUnit Unit { get; set; }

    public void Activate() => GL.ActiveTexture(Unit);

    public void Bind() {
        if (Id != boundId) {
            GL.BindTexture(TextureTarget.Texture2D, Id);
            boundId = Id;
        }
    }

    public void Delete() => GL.DeleteTexture(Id);

    public static void Unbind() {
        if (boundId != 0) {
            boundId = 0;
            GL.BindTexture(default, 0);
        }
    }

    public static Texture Create(TextureUnit unit, string name) {
        var texture = new Texture() {
            Id = GL.GenTexture(),
            Unit = unit
        };

        texture.Activate();
        texture.Bind();

        using var imageStream = EmbeddedResourceLoader.GetResourceStream(Images.Image.GetPath(name));

        using var bitmap = new Bitmap(imageStream);

        texture.Size = new Vector2(bitmap.Width, bitmap.Height);

        BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
            bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgr,
            PixelType.UnsignedByte, bitmapData.Scan0);

        bitmap.UnlockBits(bitmapData);

        return texture;
    }

    public static void SetWrapping(TextureParameterName parameter, TextureWrapMode value) {
        GL.TexParameter(TextureTarget.Texture2D, parameter, (int)value);
    }

    public static void SetMinFilter(TextureMinFilter filter) {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filter);
    }

    public static void SetMagFilter(TextureMagFilter filter) {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filter);
    }
}
