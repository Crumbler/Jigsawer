
using OpenTK.Graphics.OpenGL;

namespace Jigsawer.Main;
public struct VAO {
    private static int boundId;

    public int Id { get; private set; }

    public void Bind() {
        if (Id != boundId) {
            GL.BindVertexArray(Id);
        }
    }

    public void Delete() => GL.DeleteVertexArray(Id);

    public static void Unbind() {
        if (boundId != 0) {
            boundId = 0;
            GL.BindVertexArray(0);
        }
    }
    public static void SetVertexAttributePointer(
        int index,
        int size,
        VertexAttribPointerType type,
        bool normalized = false,
        int stride = 0,
        int offset = 0) {
        GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
    }
    public static void EnableVertexAttributeArray(int index) => GL.EnableVertexAttribArray(index);
    public static VAO Create() => new() {
        Id = GL.GenVertexArray()
    };
}
