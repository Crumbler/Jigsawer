
using OpenTK.Graphics.OpenGL;

namespace Jigsawer.Main;
public struct VAO {
    public int Id { get; private set; }

    public void Bind() => GL.BindVertexArray(Id);
    public void Delete() => GL.DeleteVertexArray(Id);

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
