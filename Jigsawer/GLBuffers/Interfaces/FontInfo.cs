
using Jigsawer.Text;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jigsawer.GLBuffers.Interfaces;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct FontInfo {
    [FieldOffset(0)]
    public float fontHeight;

    [FieldOffset(16)]
    private fixed float characterSizes[FontAtlas.TotalChars * 2];

    public Span<(float, float)> CharacterSizes {
        get {
            void* ptr = Unsafe.AsPointer(ref characterSizes[0]);
            return new Span<(float, float)>(ptr, FontAtlas.TotalChars);
        }
    }
}