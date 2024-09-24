using OpenTK.Graphics.OpenGL4;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers;

public struct VBO {
    public int Id { get; private set; }
    public BufferUsageHint Usage { get; private set; }

    [SkipLocalsInit]
    public VBO(int size) {
        int bufId;
        unsafe {
            GL.CreateBuffers(1, &bufId);
        }

        GL.NamedBufferStorage(bufId, size, 0, BufferStorageFlags.MapWriteBit);

        Id = bufId;
    }

    public void Delete() => GL.DeleteBuffer(Id);

    public unsafe void SetData<T>(T data, bool invalidate = false)
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

    public nint MapRange(int offset, int length, bool invalidate = false) {
        BufferAccessMask accessMask = BufferAccessMask.MapWriteBit;

        if (invalidate) {
            accessMask |= BufferAccessMask.MapInvalidateRangeBit;
        }

        return GL.MapNamedBufferRange(Id, offset, length, accessMask);
    }

    public void Unmap() {
        GL.UnmapNamedBuffer(Id);
    }

    private void Orphan() => GL.InvalidateBufferData(Id);
}
