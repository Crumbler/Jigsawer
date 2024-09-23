
using System.Runtime.CompilerServices;

namespace Jigsawer.Helpers;

public static class PointerExtensions {
    public static unsafe ref T ToReference<T>(this IntPtr valuePtr)
        where T : unmanaged {
        return ref Unsafe.AsRef<T>(valuePtr.ToPointer());
    }
}
