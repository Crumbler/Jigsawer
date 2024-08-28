using OpenTK.Graphics.OpenGL4;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers;

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

    public void SetData<T>(int size, T[] data)
        where T : unmanaged {
        GL.NamedBufferData(Id, size, data, Usage);
    }

    public unsafe void SetData<T>(int size, T data)
        where T : unmanaged {
        GL.NamedBufferData(Id, size, (nint)(&data), Usage);
    }

    public void Reset(int size) {
        GL.NamedBufferData(Id, size, 0, Usage);
    }

    public nint Map() {
        return GL.MapNamedBuffer(Id, BufferAccess.WriteOnly);
    }

    public void Unmap() {
        GL.UnmapNamedBuffer(Id);
    }

    public void Orphan() => GL.InvalidateBufferData(Id);

    public static void Unbind() {
        if (boundId != 0) {
            boundId = 0;
            GL.BindBuffer(default, 0);
        }
    }

    [SkipLocalsInit]
    public static VBO Create(
        BufferUsageHint usage,
        BufferTarget target = BufferTarget.ArrayBuffer) {

        int bufId;
        unsafe {
            GL.CreateBuffers(1, &bufId);
        }

        VBO buffer = new() {
            Id = bufId,
            Target = target,
            Usage = usage
        };

        return buffer;
    }
}
