
using System.Drawing;
using System.Windows.Forms;

namespace Jigsawer.Helpers;

public static class ClipboardHelper {
    private static readonly string bitmapClassName = typeof(Bitmap).FullName!;

    public static Bitmap? GetBitmap() {
        if (!Clipboard.ContainsImage()) {
            return null;
        }
        
        return Clipboard.GetDataObject()?.GetData(bitmapClassName) as Bitmap;
    }
}
