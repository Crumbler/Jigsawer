using Jigsawer.Main;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Jigsawer.Shaders.Programs;

public abstract class ShaderProgram {
    private static int currentlyUsedId;

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

    protected void SetMatrix(int location, ref Matrix2 mat) {
        GL.ProgramUniformMatrix2(Id, location, true, ref mat);
    }
    protected void SetMatrix(int location, ref Matrix3x2 mat) {
        GL.ProgramUniformMatrix3x2(Id, location, true, ref mat);
    }
    protected void SetMatrix(int location, ref Matrix2x3 mat) {
        GL.ProgramUniformMatrix2x3(Id, location, true, ref mat);
    }
    protected void SetMatrix(int location, ref Matrix3 mat) {
        GL.ProgramUniformMatrix3(Id, location, true, ref mat);
    }
    protected void SetVector2(int location, Vector2 vector) {
        GL.ProgramUniform2(Id, location, vector);
    }
    protected void SetInt(int location, int value) {
        GL.ProgramUniform1(Id, location, value);
    }

    protected void Initialize(ShaderInfo shaderInfoA, ShaderInfo shaderInfoB) {
        Shader shaderA = shaderInfoA.Load(),
            shaderB = shaderInfoB.Load();

        Id = GL.CreateProgram();

        AttachShader(shaderA);
        AttachShader(shaderB);

        Link();

        Validate();

        DetachShader(shaderA);
        DetachShader(shaderB);

        shaderA.Delete();
        shaderB.Delete();
    }

    public static void StopUsing() {
        if (currentlyUsedId != 0) {
            currentlyUsedId = 0;
            GL.UseProgram(0);
        }
    }

    public void Use() {
        if (currentlyUsedId != Id) {
            GL.UseProgram(Id);
            currentlyUsedId = Id;
        }
    }

    public void Delete() => GL.DeleteProgram(Id);
}
