using OpenTK.Graphics.OpenGL4;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers;

public struct UBO {
    private readonly int id;
    private readonly int bindingPoint;

    [SkipLocalsInit]
    public UBO(int bindingPoint, int size) {
        int bufId;
        unsafe {
            GL.CreateBuffers(1, &bufId);
        }

        GL.NamedBufferStorage(bufId, size, 0, BufferStorageFlags.MapWriteBit);

        id = bufId;
        this.bindingPoint = bindingPoint;
    }

    public void Bind() {
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, bindingPoint, id);
    }

    public unsafe ref T Map<T>() where T : unmanaged {
        IntPtr handle =  GL.MapNamedBuffer(id, BufferAccess.WriteOnly);
        return ref Unsafe.AsRef<T>((void*)handle);
    }

    public void Unmap() {
        GL.UnmapNamedBuffer(id);
    }

    public void Delete() => GL.DeleteBuffer(id);
}
