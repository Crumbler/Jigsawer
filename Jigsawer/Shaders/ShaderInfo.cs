
using Jigsawer.Resources;
using OpenTK.Graphics.OpenGL;

namespace Jigsawer.Shaders;

public struct ShaderInfo {
    public string SourcePath { get; private set; }
    public ShaderType Type { get; private set; }

    public ShaderInfo(string sourcePath, ShaderType type) {
        SourcePath = sourcePath;
        Type = type;
    }

    public static ShaderInfo Get(string entityName, ShaderType type) => 
        new(Shader.GetEntityPath(entityName, type), type);

    public Shader Load() {
        var shader = Shader.Create(Type);
        
        string shaderSourceCode = EmbeddedResourceLoader.GetResourceString(SourcePath);

        shader.SetSource(shaderSourceCode);

        shader.Compile();

        return shader;
    }
}
