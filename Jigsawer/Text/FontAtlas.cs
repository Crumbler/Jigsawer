
using Jigsawer.GLObjects;

using OpenTK.Graphics.OpenGL4;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Jigsawer.Text;

public sealed partial class FontAtlas {
    private const string FontFamilyName = "MS Gothic";
    // 94 printable ASCII characters
    public const int MinChar = '!', MaxChar = '~', TotalChars = MaxChar - MinChar + 1;

    private static readonly Graphics measureGraphics = Graphics.FromHdc(CreateCompatibleDC(0));
    public static readonly FontAtlas Normal = new(FontFamilyName, 20);

    static FontAtlas() {
        measureGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
    }

    public Texture Texture { get; private set; }
    public int CharacterHeight { get; private set; }
    public float SpaceWidth { get; private set; }
    public float EmSize { get; private set; }
    public ReadOnlySpan<float> CharacterWidths => characterWidths;
    private readonly float[] characterWidths = new float[TotalChars];

    private FontAtlas(string fontFamily, int emSize) {
        EmSize = emSize;
        using var font = new Font(fontFamily, emSize);

        CharacterHeight = font.Height;

        CalculateCharacterWidths(font, out float maxCharacterWidth);

        int bitmapWidth = (int)MathF.Ceiling(maxCharacterWidth);
        int bitmapHeight = TotalChars * font.Height;

        using var bitmap = new Bitmap(bitmapWidth, bitmapHeight,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        using var g = Graphics.FromImage(bitmap);

        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        g.FillRectangle(Brushes.White, 0, 0, bitmapWidth, bitmapHeight);

        Span<char> charSpan = stackalloc char[1];

        for (int i = MinChar; i <= MaxChar; ++i) {
            charSpan[0] = (char)i;

            int y = (i - MinChar) * font.Height;

            g.DrawString(charSpan, font, Brushes.Black, 0f, y, StringFormat.GenericTypographic);

            float characterWidth = measureGraphics.MeasureString(charSpan,
                font, int.MaxValue, StringFormat.GenericTypographic).Width;
        }

        GenerateTexture(bitmap);
    }

    public override int GetHashCode() {
        return EmSize.GetHashCode();
    }

    private void GenerateTexture(Bitmap bitmap) {
        Texture = new Texture();
        Texture.SetMinFilter(TextureMinFilter.Linear);
        Texture.SetMagFilter(TextureMagFilter.Linear);

        BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        GL.TextureStorage2D(Texture.Id, 1, SizedInternalFormat.R8, bitmap.Width, bitmap.Height);
        GL.TextureSubImage2D(Texture.Id, 0, 0, 0, bitmap.Width, bitmap.Height,
            OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.UnsignedByte, bitmapData.Scan0);

        bitmap.UnlockBits(bitmapData);
    }

    private void CalculateCharacterWidths(Font font, out float maxCharacterWidth) {
        SpaceWidth = measureGraphics.MeasureString([' ', 'a'], font,
            int.MaxValue, StringFormat.GenericTypographic).Width -
                    measureGraphics.MeasureString(['a'], font,
            int.MaxValue, StringFormat.GenericTypographic).Width;

        float maxWidth = 0f;

        Span<char> charSpan = stackalloc char[1];

        Span<float> charWidths = characterWidths;

        for (int i = MinChar; i <= MaxChar; ++i) {
            charSpan[0] = (char)i;

            float width = measureGraphics.MeasureString(charSpan,
                font, int.MaxValue, StringFormat.GenericTypographic).Width;
            charWidths[i - MinChar] = width;

            maxWidth = MathF.Max(maxWidth, width);
        }

        maxCharacterWidth = maxWidth;
    }

    [LibraryImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial IntPtr CreateCompatibleDC(IntPtr hdc);
}
