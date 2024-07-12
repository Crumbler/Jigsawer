
using Jigsawer.Main;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Jigsawer.Models;

public sealed class ImageModel : RenderModel {
    private readonly Vector2[] vertexCoords;
    private readonly VAO vao;
    private readonly VBO positionVBO;

    public Box2 Rect {
        get => new(vertexCoords[0], vertexCoords[2]);

        set {
            float left = value.Min.X;
            float bottom = value.Min.Y;
            float right = value.Max.X;
            float top = value.Max.Y;

            vertexCoords[0] = new Vector2(left, bottom);
            vertexCoords[1] = new Vector2(left, top);
            vertexCoords[2] = new Vector2(right, top);
            vertexCoords[3] = new Vector2(right, bottom);

            positionVBO.SetData<Vector2>(vertexCoords.Length * Vector2.SizeInBytes, null);
            positionVBO.SetData(vertexCoords.Length * Vector2.SizeInBytes, vertexCoords);
        }
    }

    public ImageModel() {
        vertexCoords = new Vector2[4];

        vao = VAO.Create();
        vao.Bind();

        positionVBO = VBO.Create(BufferUsageHint.DynamicDraw);
        positionVBO.Bind();

        positionVBO.SetData(vertexCoords.Length * Vector2.SizeInBytes, vertexCoords);

        VAO.SetVertexAttributePointer(0, 2, VertexAttribPointerType.Float);

        VAO.EnableVertexAttributeArray(0);
    }

    public void Render() {
        vao.Bind();
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, vertexCoords.Length);
    }

    public override void Delete() {
        vao.Delete();
        positionVBO.Delete();
    }
}
