
using Jigsawer.Resources;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Drawing;
using System.Drawing.Imaging;

namespace Jigsawer.Main;

public struct Texture {
    public int Id { get; private set; }
    public int Unit { get; private set; }

    public Texture() {
        Initialize();
    }

    public Texture(string resourceName) {
        Initialize();

        using var imageStream = EmbeddedResourceLoader.GetResourceStream(Images.Image.GetPath(resourceName));

        using var bitmap = new Bitmap(imageStream);

        BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        GL.TextureStorage2D(Id, 1, SizedInternalFormat.Rgb8, bitmap.Width, bitmap.Height);
        GL.TextureSubImage2D(Id, 0, 0, 0, bitmap.Width, bitmap.Height,
            OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.UnsignedByte, bitmapData.Scan0);

        bitmap.UnlockBits(bitmapData);
    }

    private void Initialize() {
        GL.CreateTextures(TextureTarget.Texture2D, 1, out int textureId);

        Id = textureId;
        Unit = TextureUnits.GrabUnit();
    }

    public void Use() {
        TextureUnits.Bind(Unit, Id);
    }

    public void Delete() {
        TextureUnits.ReturnUnit(Unit);
        GL.DeleteTexture(Id);
    }

    public static void Unbind() {
        TextureUnits.UnbindAll();
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
