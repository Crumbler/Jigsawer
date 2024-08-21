
using Jigsawer.Exceptions;
using Jigsawer.Main;

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Shaders;

public struct Shader {
    public int Id { get; private set; }

    private readonly bool CompilationSuccessful => GetBool(ShaderParameter.CompileStatus);
    private readonly string InfoLog => GL.GetShaderInfoLog(Id);

    public static Shader Create(ShaderType type) => new() {
        Id = GL.CreateShader(type)
    };

    public void SetSource(string sourceCode) => GL.ShaderSource(Id, sourceCode);

    public void Compile() {
        GL.CompileShader(Id);

        if (!CompilationSuccessful) {
            Logger.LogError("Shader compilation error:\n" + InfoLog);
        }
    }

    public void Delete() => GL.DeleteShader(Id);

    private readonly bool GetBool(ShaderParameter parameter) {
        GL.GetShader(Id, parameter, out int result);

        return result != 0;
    }

    public static string GetEntityPath(string entityName, ShaderType shaderType) {
        string shaderTypeName = shaderType switch {
            ShaderType.VertexShader => "vert",
            ShaderType.FragmentShader => "frag",
            _ => throw new ShaderTypeNotSupportedException(shaderType)
        };

        return $"Jigsawer.Shaders.Entities.{entityName}.{shaderTypeName}.glsl";
    }
}
