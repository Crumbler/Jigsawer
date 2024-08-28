
using Jigsawer.Text;

using System.Runtime.InteropServices;

namespace Jigsawer.GLBuffers.Interfaces;

public unsafe struct FontInfo {
    public float fontHeight;
    private fixed float characterWidths[FontAtlas.TotalChars];

    public Span<float> CharacterWidths => 
        MemoryMarshal.CreateSpan(ref characterWidths[0], FontAtlas.TotalChars);
}
