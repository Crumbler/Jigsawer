

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Exceptions;

public sealed class UnknownDebugSeverityException : Exception {
    public UnknownDebugSeverityException(DebugSeverity severity) : 
        base("Unknown debug severity: " + severity) { }
}
