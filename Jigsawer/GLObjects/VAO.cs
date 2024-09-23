using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.GLObjects;
public struct VAO {
    private static int boundId;

    public int Id { get; private set; }

    public VAO() {
        GL.CreateVertexArrays(1, out int vaoId);
        Id = vaoId;
    }

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

    public void SetBindingPointToBuffer(int bindingPoint, int bufferId, int offset = 0, int stride = 0) {
        GL.VertexArrayVertexBuffer(Id, bindingPoint, bufferId, offset, stride);
    }

    public void BindAttributeToPoint(int attribute, int bindingPoint) {
        GL.VertexArrayAttribBinding(Id, attribute, bindingPoint);
    }

    public void SetAttributeFormat(
        int attribute,
        int size,
        VertexAttribType type,
        bool normalized = false,
        int offset = 0) {
        GL.VertexArrayAttribFormat(Id, attribute, size, type, normalized, offset);
    }

    public void SetIntegerAttributeFormat(
        int attribute,
        int size,
        VertexAttribIntegerType type) {
        GL.VertexArrayAttribIFormat(Id, attribute, size, type, 0);
    }

    public void SetBindingPointDivisor(int bindingPoint, int divisor) {
        GL.VertexArrayBindingDivisor(Id, bindingPoint, divisor);
    }

    public void EnableVertexAttributeArray(int index) => GL.EnableVertexArrayAttrib(Id, index);
}
