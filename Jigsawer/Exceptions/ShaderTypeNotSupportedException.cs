
using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Exceptions;
public sealed class ShaderTypeNotSupportedException : Exception {
    public ShaderTypeNotSupportedException(ShaderType shaderType) :
        base($"Shader type {shaderType} not supported.") { }
}
