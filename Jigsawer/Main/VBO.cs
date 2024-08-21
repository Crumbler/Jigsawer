
using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Main;

public struct VBO {
    private static int boundId;

    public int Id { get; private set; }
    public BufferTarget Target { get; private set; }
    public BufferUsageHint Usage { get; private set; }

    public void Bind() {
        if (boundId != Id) {
            GL.BindBuffer(Target, Id);
            boundId = Id;
        }
    }

    public void Delete() => GL.DeleteBuffer(Id);
    public void SetData<T>(int size, T[]? data)
        where T : unmanaged {
        GL.BufferData(Target, size, data, Usage);
    }
    public unsafe void SetData<T>(int size, T data)
        where T : unmanaged {
        GL.BufferData(Target, size, (nint)(&data), Usage);
    }

    public void Orphan(int size) => GL.BufferData(Target, size, 0, Usage);

    public static void Unbind() {
        if (boundId != 0) {
            boundId = 0;
            GL.BindBuffer(default, 0);
        }
    }

    public static VBO Create(
        BufferUsageHint usage,
        BufferTarget target = BufferTarget.ArrayBuffer) => new() {
        Id = GL.GenBuffer(),
        Target = target,
        Usage = usage
    };
}
