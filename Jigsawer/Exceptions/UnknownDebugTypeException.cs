

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Exceptions;

public sealed class UnknownDebugTypeException : Exception {
    public UnknownDebugTypeException(DebugType type) : base("Unknown debug type: " + type) { }
}
