using Jigsawer.Main;

using OpenTK.Graphics.OpenGL;

namespace Jigsawer.Shaders.Programs;

public abstract class ShaderProgram {
    public int Id {
        get; private set;
    }

    private bool LinkSuccessful => GetBool(GetProgramParameterName.LinkStatus);
    private bool ValidationSuccessful => GetBool(GetProgramParameterName.ValidateStatus);
    private string InfoLog => GL.GetProgramInfoLog(Id);

    private bool GetBool(GetProgramParameterName parameter) {
        GL.GetProgram(Id, parameter, out int success);

        return success != 0;
    }

    private void AttachShader(Shader shader) => GL.AttachShader(Id, shader.Id);
    private void DetachShader(Shader shader) => GL.DetachShader(Id, shader.Id);

    private void Link() {
        GL.LinkProgram(Id);

        if (!LinkSuccessful) {
            Logger.LogError("Program linking error:\n" + InfoLog);
        }
    }

    private void Validate() {
        GL.ValidateProgram(Id);

        if (!ValidationSuccessful) {
            Logger.LogError("Program validation error:\n" + InfoLog);
        }
    }

    protected void BindAttribute(int index, string name) => GL.BindAttribLocation(Id, index, name);
    protected abstract void BindAttributes();

    protected void Initialize(ShaderInfo shaderInfoA, ShaderInfo shaderInfoB) {
        Shader shaderA = shaderInfoA.Load(),
            shaderB = shaderInfoB.Load();

        Id = GL.CreateProgram();

        AttachShader(shaderA);
        AttachShader(shaderB);

        BindAttributes();

        Link();

        Validate();

        DetachShader(shaderA);
        DetachShader(shaderB);

        shaderA.Delete();
        shaderB.Delete();
    }

    public void Use() => GL.UseProgram(Id);
    public void Delete() => GL.DeleteProgram(Id);
}
