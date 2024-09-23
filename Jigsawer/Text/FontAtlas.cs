
using Jigsawer.GLObjects;
using Jigsawer.Helpers;

using OpenTK.Graphics.OpenGL4;

using SharpFont;

namespace Jigsawer.Text;

public sealed record FontMetrix(float Height, float Ascender);

public sealed partial class FontAtlas {
    private const string FontName = "Ebrima";
    // 94 printable ASCII characters
    public const char MinChar = '!', MaxChar = '~';
    public const int TotalChars = MaxChar - MinChar + 1;

    private static readonly List<FontAtlas> atlases = GenerateFontAtlases();

    private static List<FontAtlas> GenerateFontAtlases() {
        using var library = new Library();

        string fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

        using var face = new Face(library, Path.Combine(fontsFolder, FontName + ".ttf"));

        var atlases = new List<FontAtlas>();

        const int StartSize = 8, SizeStep = 8, EndSize = StartSize + SizeStep * 3;

        int originalAlignment = PixelParameters.UnpackAlignment;
        PixelParameters.UnpackAlignment = 1;

        for (int size = StartSize; size <= EndSize; size += SizeStep) {
            atlases.Add(new FontAtlas(face, size));
        }

        PixelParameters.UnpackAlignment = originalAlignment;

        return atlases;
    }

    public static FontAtlas GetFontAtlas(int emSize) =>
        atlases.MinBy(atlas => int.Abs(atlas.EmSize - emSize))!;

    public Texture Texture { get; private set; }
    public int CharacterHeight { get; private set; }
    public float SpaceAdvance { get; private set; }
    public float MaxAscender { get; private set; }
    public int EmSize { get; private set; }
    public ReadOnlySpan<(float width, float height)> CharacterSizes => characterSizes;
    public ReadOnlySpan<(int bearingX, int bearingY, float advance)> CharacterMetrics => characterMetrics;

    private readonly (float width, float height)[] characterSizes =
        new (float, float)[TotalChars];
    private readonly (int bearingX, int bearingY, float advance)[] characterMetrics =
        new (int, int, float)[TotalChars];

    private FontAtlas(Face face, int emSize) {
        face.SetPixelSizes(0, (uint)emSize);

        EmSize = emSize;

        CharacterHeight = (int)face.Size.Metrics.Height;
        MaxAscender = (float)face.Size.Metrics.Ascender;

        GenerateTextureAndCalculateMetrics(face);
    }

    public override int GetHashCode() {
        return EmSize.GetHashCode();
    }

    private void GenerateTextureAndCalculateMetrics(Face face) {
        Texture = new Texture();
        Texture.SetMinFilter(TextureMinFilter.Linear);
        Texture.SetMagFilter(TextureMagFilter.Linear);

        Texture.SetStorage2D(1, SizedInternalFormat.R8,
            face.Size.Metrics.NominalWidth, CharacterHeight * TotalChars);

        Span<(float width, float height)> sizesSpan = characterSizes;
        Span<(int bearingX, int bearingY, float advance)> metricsSpan = characterMetrics;

        for (char c = MinChar; c <= MaxChar; ++c) {
            face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);

            int ind = c - MinChar;

            var glyph = face.Glyph;

            sizesSpan[ind] = ((float)glyph.Metrics.Width, (float)glyph.Metrics.Height);
            metricsSpan[ind] = (glyph.BitmapLeft, glyph.BitmapTop, (float)glyph.Advance.X);

            int y = CharacterHeight * ind;
            int width = glyph.Bitmap.Width;
            int height = glyph.Bitmap.Rows;

            Texture.SetSubImage2D(0, 0, y, width, height,
                PixelFormat.Red, PixelType.UnsignedByte, glyph.Bitmap.Buffer);
        }

        face.LoadChar(' ', LoadFlags.Default, LoadTarget.Normal);
        SpaceAdvance = (float)face.Glyph.Metrics.HorizontalAdvance;
    }
}
