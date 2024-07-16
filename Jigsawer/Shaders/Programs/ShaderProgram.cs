using Jigsawer.Main;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

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

    protected static void SetMatrix(int location, ref Matrix2 mat) {
        GL.UniformMatrix2(location, true, ref mat);
    }
    protected static void SetMatrix(int location, ref Matrix3x2 mat) {
        GL.UniformMatrix3x2(location, true, ref mat);
    }
    protected static void SetMatrix(int location, ref Matrix2x3 mat) {
        GL.UniformMatrix2x3(location, true, ref mat);
    }
    protected static void SetMatrix(int location, ref Matrix3 mat) {
        GL.UniformMatrix3(location, true, ref mat);
    }

    protected void BindAttribute(int index, string name) => GL.BindAttribLocation(Id, index, name);
    protected int GetUniformLocation(string name) => GL.GetUniformLocation(Id, name);

    protected abstract void GetUniformLocations();
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

        GetUniformLocations();
    }

    public void Use() => GL.UseProgram(Id);
    public void Delete() => GL.DeleteProgram(Id);
}
