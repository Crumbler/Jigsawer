using Jigsawer.Resources;

using OpenTK.Graphics.OpenGL4;

using System.Drawing;
using System.Drawing.Imaging;

namespace Jigsawer.GLObjects;

public record struct TextureParameters(TextureMinFilter MinFilter,
    TextureMagFilter MagFilter,
    TextureWrapMode HorWrap,
    TextureWrapMode VertWrap);

public struct Texture {
    public static readonly TextureParameters defaultParameters =
        new(TextureMinFilter.Linear,
            TextureMagFilter.Linear,
            TextureWrapMode.ClampToEdge,
            TextureWrapMode.ClampToEdge),
        repeatingParameters =
        new(TextureMinFilter.Linear,
            TextureMagFilter.Linear,
            TextureWrapMode.Repeat,
            TextureWrapMode.Repeat);
    public int Id { get; private set; }
    public int Unit { get; private set; }

    public Texture() {
        Initialize();
    }

    public Texture(Bitmap bitmap) {
        Initialize();

        CopyFromBitmap(bitmap);
    }

    public Texture(string resourceName) {
        Initialize();

        using var imageStream =
            EmbeddedResourceLoader.GetResourceStream(Images.EmbeddedImage.GetPath(resourceName));

        using var bitmap = new Bitmap(imageStream);

        CopyFromBitmap(bitmap);
    }

    private void CopyFromBitmap(Bitmap bitmap) {
        BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        SetStorage2D(1, SizedInternalFormat.Rgb8, bitmap.Width, bitmap.Height);
        SetSubImage2D(0, 0, 0, bitmap.Width, bitmap.Height,
            OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

        bitmap.UnlockBits(bitmapData);
    }

    public void SetStorage2D(int levels, SizedInternalFormat internalFormat,
        int width, int height) {
        GL.TextureStorage2D(Id, levels, internalFormat, width, height);
    }

    public void SetSubImage2D(int level, int x, int y, int width, int height,
        OpenTK.Graphics.OpenGL4.PixelFormat pixelFormat, PixelType type, nint data) {
        GL.TextureSubImage2D(Id, level, x, y, width, height,
            pixelFormat, type, data);
    }

    private void Initialize() {
        GL.CreateTextures(TextureTarget.Texture2D, 1, out int textureId);

        Id = textureId;
        Unit = TextureUnits.GrabOne();
    }

    public void Use() {
        TextureUnits.Bind(Unit, Id);
    }

    public void Delete() {
        TextureUnits.ReturnOne(Unit);
        GL.DeleteTexture(Id);
    }

    public static void Unbind() {
        TextureUnits.UnbindAll();
    }

    public void SetParameters(TextureParameters parameters) {
        SetMinFilter(parameters.MinFilter);
        SetMagFilter(parameters.MagFilter);
        SetWrapping(TextureParameterName.TextureWrapS, parameters.HorWrap);
        SetWrapping(TextureParameterName.TextureWrapT, parameters.VertWrap);
    }

    public void SetWrapping(TextureParameterName parameter, TextureWrapMode value) {
        GL.TextureParameter(Id, parameter, (int)value);
    }

    public void SetMinFilter(TextureMinFilter filter) {
        GL.TextureParameter(Id, TextureParameterName.TextureMinFilter, (int)filter);
    }

    public void SetMagFilter(TextureMagFilter filter) {
        GL.TextureParameter(Id, TextureParameterName.TextureMagFilter, (int)filter);
    }
}
