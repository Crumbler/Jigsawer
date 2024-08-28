using OpenTK.Graphics.OpenGL4;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers;

public struct UBO {
    public int Id { get; private set; }
    private int bindingPoint;

    private UBO(int id, int bindingPoint) {
        Id = id;
        this.bindingPoint = bindingPoint;
    }

    public void Bind() {
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, bindingPoint, Id);
    }

    public nint Map() {
        return GL.MapNamedBuffer(Id, BufferAccess.WriteOnly);
    }

    public void Unmap() {
        GL.UnmapNamedBuffer(Id);
    }

    public void Delete() => GL.DeleteBuffer(Id);

    [SkipLocalsInit]
    public static UBO Create(int bindingPoint, int size) {
        int bufId;
        unsafe {
            GL.CreateBuffers(1, &bufId);
        }

        GL.NamedBufferStorage(bufId, size, 0, BufferStorageFlags.MapWriteBit);

        return new UBO(bufId, bindingPoint);
    }
}
