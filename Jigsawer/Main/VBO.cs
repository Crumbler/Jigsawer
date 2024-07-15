
using OpenTK.Graphics.OpenGL;

namespace Jigsawer.Main;

public struct VBO {
    public int Id { get; private set; }
    public BufferTarget Target { get; private set; }
    public BufferUsageHint Usage { get; private set; }

    public void Bind() => GL.BindBuffer(Target, Id);
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

    public static VBO Create(
        BufferUsageHint usage,
        BufferTarget target = BufferTarget.ArrayBuffer) => new() {
        Id = GL.GenBuffer(),
        Target = target,
        Usage = usage
    };
}
