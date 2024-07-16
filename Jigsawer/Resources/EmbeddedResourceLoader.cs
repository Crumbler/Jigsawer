
using Jigsawer.Exceptions;

using System.Reflection;
using System.Text;

namespace Jigsawer.Resources;

public static class EmbeddedResourceLoader {
    private static readonly Assembly assembly = typeof(EmbeddedResourceLoader).Assembly;

    public static Stream GetResourceStream(string resourceName) {
        Stream? stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null) {
            throw new ResourceNotFoundException(resourceName);
        }

        return stream;
    }

    public static string GetResourceString(string resourceName) {
        using var stream = GetResourceStream(resourceName);

        using var reader = new StreamReader(stream, Encoding.UTF8);

        return reader.ReadToEnd();
    }
}
