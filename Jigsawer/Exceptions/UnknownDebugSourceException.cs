
using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Exceptions;

public sealed class UnknownDebugSourceException : Exception {
    public UnknownDebugSourceException(DebugSource source) : base("Unknown debug source: " + source) { }
}
