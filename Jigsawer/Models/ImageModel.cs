
using Jigsawer.Main;
using static Jigsawer.Shaders.Programs.ImageShaderProgram;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public sealed class ImageModel : RenderModel {
    private const int PrimitivesPerInstance = 4;
    private const int InstanceDataSize = sizeof(float) * PrimitivesPerInstance;

    private Box2 box;
    private readonly VAO vao;
    private readonly VBO positionVBO;

    public Box2 Rect {
        get => box;

        set {
            box = value;

            positionVBO.Bind();
            positionVBO.Orphan(InstanceDataSize);
            positionVBO.SetData(InstanceDataSize, value);
        }
    }

    public ImageModel() {
        vao = VAO.Create();
        vao.Bind();

        positionVBO = VBO.Create(BufferUsageHint.DynamicDraw);
        positionVBO.Bind();

        positionVBO.SetData(InstanceDataSize, box);

        VAO.SetVertexAttributePointer(AttributePositions.Position,
            PrimitivesPerInstance, VertexAttribPointerType.Float);
        GL.VertexAttribDivisor(AttributePositions.Position, PrimitivesPerInstance);
        VAO.EnableVertexAttributeArray(AttributePositions.Position);
    }

    public void Render() {
        vao.Bind();

        GL.DrawArrays(PrimitiveType.TriangleFan, 0, PrimitivesPerInstance);
    }

    public override void Delete() {
        vao.Delete();
        positionVBO.Delete();
    }
}
