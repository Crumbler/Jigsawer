using static Jigsawer.Shaders.Programs.ImageShaderProgram;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Jigsawer.Shaders.Programs;
using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;

namespace Jigsawer.Models;

public sealed class ImageModel : IRenderableModel {
    private const int PrimitivesPerInstance = 4;
    private const int InstanceDataSize = sizeof(float) * PrimitivesPerInstance;

    private Box2 box;
    private readonly VAO vao;
    private readonly VBO positionVBO;
    private readonly Texture texture;
    private readonly ImageShaderProgram shader;

    public Box2 Rect {
        get => box;

        set {
            box = value;

            positionVBO.SetData(value, true);
        }
    }

    public ImageModel(int sharedInfoUboBindingPoint, float scaleFactor) {
        positionVBO = new VBO(InstanceDataSize);
        positionVBO.SetData(box);

        vao = new VAO();
        vao.SetBindingPointToBuffer(0, positionVBO.Id);
        vao.SetBindingPointDivisor(0, 1);

        vao.EnableVertexAttributeArray(AttributePositions.Position);
        vao.BindAttributeToPoint(AttributePositions.Position, 0);
        vao.SetAttributeFormat(AttributePositions.Position,
            PrimitivesPerInstance, VertexAttribType.Float);

        texture = new Texture(Images.Image.MainMenuBackgroundTile);
        texture.SetMinFilter(TextureMinFilter.Linear);
        texture.SetMagFilter(TextureMagFilter.Linear);
        texture.SetWrapping(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat);
        texture.SetWrapping(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat);

        shader = new ImageShaderProgram(sharedInfoUboBindingPoint);
        shader.SetScaleFactor(scaleFactor);
        shader.SetTextureUnit(texture.Unit);
    }

    public void Render() {
        vao.Bind();

        texture.Use();

        shader.Use();

        GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, PrimitivesPerInstance, 1);
    }

    public void Delete() {
        vao.Delete();
        positionVBO.Delete();
        texture.Delete();
    }
}
