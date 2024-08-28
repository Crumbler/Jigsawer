﻿using OpenTK.Graphics.OpenGL4;

using System.Runtime.CompilerServices;

namespace Jigsawer.GLBuffers;

public struct UBO<T> where T : unmanaged {
    private readonly int id;
    private readonly int bindingPoint;

    [SkipLocalsInit]
    public UBO(int bindingPoint) {
        int bufId;
        unsafe {
            GL.CreateBuffers(1, &bufId);
        }

        GL.NamedBufferStorage(bufId, Unsafe.SizeOf<T>(), 0, BufferStorageFlags.MapWriteBit);

        id = bufId;
        this.bindingPoint = bindingPoint;
    }

    public void Bind() {
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, bindingPoint, id);
    }

    public unsafe ref T Map() {
        IntPtr handle =  GL.MapNamedBuffer(id, BufferAccess.WriteOnly);
        return ref Unsafe.AsRef<T>((void*)handle);
    }

    public void Unmap() {
        GL.UnmapNamedBuffer(id);
    }

    public void Delete() => GL.DeleteBuffer(id);
}
