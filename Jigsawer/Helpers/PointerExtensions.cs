
using System.Runtime.CompilerServices;

namespace Jigsawer.Helpers;

public static class PointerExtensions {
    public static unsafe ref T ToReference<T>(this IntPtr valuePtr)
        where T : unmanaged {
        return ref Unsafe.AsRef<T>((void*)valuePtr);
    }

    public static unsafe Span<T> ToSpan<T>(this IntPtr spanPtr, int elementCount)
        where T : unmanaged {
        return new Span<T>((void*)spanPtr, elementCount);
    }
}
