
using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Main;
public struct VAO {
    private static int boundId;

    public int Id { get; private set; }

    public void Bind() {
        if (Id != boundId) {
            GL.BindVertexArray(Id);
            boundId = Id;
        }
    }

    public void Delete() => GL.DeleteVertexArray(Id);

    public static void Unbind() {
        if (boundId != 0) {
            boundId = 0;
            GL.BindVertexArray(0);
        }
    }

    public void SetBindingPointToBuffer(int bindingPoint, int bufferId) {
        GL.VertexArrayVertexBuffer(Id, bindingPoint, bufferId, 0, 0);
    }

    public void BindAttributeToPoint(int attribute, int bindingPoint) {
        GL.VertexArrayAttribBinding(Id, attribute, bindingPoint);
    }

    public void SetAttributeFormat(
        int attribute,
        int size,
        VertexAttribType type) {
        GL.VertexArrayAttribFormat(Id, attribute, size, type, false, 0);
    }

    public void SetBindingPointDivisor(int bindingPoint, int divisor) {
        GL.VertexArrayBindingDivisor(Id, bindingPoint, divisor);
    }

    public void EnableVertexAttributeArray(int index) => GL.EnableVertexArrayAttrib(Id, index);

    public static VAO Create() {
        GL.CreateVertexArrays(1, out int vaoId);

        return new VAO() {
            Id = vaoId
        };
    }
}
