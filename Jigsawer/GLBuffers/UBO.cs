using Jigsawer.GLObjects;

using OpenTK.Graphics.OpenGL4;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers;

public struct UBO<T> where T : unmanaged {
    private readonly int id;

    public int BindingPoint { get; private set; }

    [SkipLocalsInit]
    public UBO() {
        int bufId;
        unsafe {
            GL.CreateBuffers(1, &bufId);
        }

        GL.NamedBufferStorage(bufId, Unsafe.SizeOf<T>(), 0, BufferStorageFlags.MapWriteBit);

        id = bufId;
        BindingPoint = BufferBindingPoints.GrabUBOPoint();
    }

    public void Bind() {
        BufferBindingPoints.BindUBO(BindingPoint, id);
    }

    public static void UnbindAll() {
        BufferBindingPoints.UnbindAllUBOs();
    }

    public unsafe ref T Map() {
        IntPtr handle =  GL.MapNamedBuffer(id, BufferAccess.WriteOnly);
        return ref Unsafe.AsRef<T>((void*)handle);
    }

    public void Unmap() {
        GL.UnmapNamedBuffer(id);
    }

    public void Delete() {
        BufferBindingPoints.ReturnUBOPoint(BindingPoint);
        GL.DeleteBuffer(id);
    }
}
