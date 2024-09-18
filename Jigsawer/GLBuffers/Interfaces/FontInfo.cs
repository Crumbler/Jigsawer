
using Jigsawer.Text;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers.Interfaces;

public unsafe struct FontInfo {
    public float fontHeight;
    // padding to compensate for std140 layout
#pragma warning disable CA1823 // Avoid unused private fields
    private fixed float _padding[3];
#pragma warning restore CA1823 // Avoid unused private fields

    private fixed float characterSizes[FontAtlas.TotalChars * 2];

    public Span<(float, float)> CharacterSizes {
        get {
            void* ptr = Unsafe.AsPointer(ref characterSizes[0]);
            return new Span<(float, float)>(ptr, FontAtlas.TotalChars);
        }
    }
}