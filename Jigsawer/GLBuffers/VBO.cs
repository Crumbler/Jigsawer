using OpenTK.Graphics.OpenGL4;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers;

public struct VBO {
    private static int boundId;

    public int Id { get; private set; }
    public BufferTarget Target { get; private set; }
    public BufferUsageHint Usage { get; private set; }

    [SkipLocalsInit]
    public VBO(
        int size,
        BufferTarget target = BufferTarget.ArrayBuffer) {
        int bufId;
        unsafe {
            GL.CreateBuffers(1, &bufId);
        }

        GL.NamedBufferStorage(bufId, size, 0, BufferStorageFlags.MapWriteBit);

        Id = bufId;
        Target = target;
    }

    public void Bind() {
        if (boundId != Id) {
            GL.BindBuffer(Target, Id);
            boundId = Id;
        }
    }

    public void Delete() => GL.DeleteBuffer(Id);

    public unsafe void SetData<T>(int size, T data, bool invalidate = false)
        where T : unmanaged {
        if (invalidate) {
            Orphan();
        }

        var dataPtr = (T*)GL.MapNamedBuffer(Id, BufferAccess.WriteOnly);
        *dataPtr = data;
        GL.UnmapNamedBuffer(Id);
    }

    public nint Map(bool invalidate = false) {
        if (invalidate) {
            Orphan();
        }

        return GL.MapNamedBuffer(Id, BufferAccess.WriteOnly);
    }

    public void Unmap() {
        GL.UnmapNamedBuffer(Id);
    }

    private void Orphan() => GL.InvalidateBufferData(Id);

    public static void Unbind() {
        if (boundId != 0) {
            boundId = 0;
            GL.BindBuffer(default, 0);
        }
    }
}
