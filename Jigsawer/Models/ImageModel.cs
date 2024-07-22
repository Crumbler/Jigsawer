
using Jigsawer.Main;
using static Jigsawer.Shaders.Programs.ImageShaderProgram;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Jigsawer.Shaders.Programs;

namespace Jigsawer.Models;

public sealed class ImageModel {
    private const int PrimitivesPerInstance = 4;
    private const int InstanceDataSize = sizeof(float) * PrimitivesPerInstance;

    private Box2 box;
    private readonly VAO vao;
    private readonly VBO positionVBO;
    private readonly Texture texture;
    private readonly ImageShaderProgram imageShader;

    public Box2 Rect {
        get => box;

        set {
            box = value;

            positionVBO.Bind();
            positionVBO.Orphan(InstanceDataSize);
            positionVBO.SetData(InstanceDataSize, value);
        }
    }

    public ImageModel(ref Matrix3 projMat, float textureSizeMultiplier) {
        vao = VAO.Create();
        vao.Bind();

        positionVBO = VBO.Create(BufferUsageHint.DynamicDraw);
        positionVBO.Bind();

        positionVBO.SetData(InstanceDataSize, box);

        VAO.SetVertexAttributePointer(AttributePositions.Position,
            PrimitivesPerInstance, VertexAttribPointerType.Float);
        GL.VertexAttribDivisor(AttributePositions.Position, PrimitivesPerInstance);
        VAO.EnableVertexAttributeArray(AttributePositions.Position);

        texture = Texture.Create(TextureUnit.Texture0, Images.Image.MainMenuBackgroundTile);

        imageShader = ImageShaderProgram.Create();
        imageShader.Use();
        imageShader.SetProjectionMatrix(ref projMat);
        imageShader.SetTextureSize(texture.Size * textureSizeMultiplier);
        imageShader.SetTextureUnit(0);
    }

    public void UpdateProjectionMatrix(ref Matrix3 mat) {
        imageShader.Use();
        imageShader.SetProjectionMatrix(ref mat);
    }

    public void Render() {
        vao.Bind();

        texture.Activate();
        texture.Bind();
        Texture.SetMinFilter(TextureMinFilter.Linear);
        Texture.SetMagFilter(TextureMagFilter.Linear);
        Texture.SetWrapping(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat);
        Texture.SetWrapping(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat);

        imageShader.Use();

        GL.DrawArrays(PrimitiveType.TriangleFan, 0, PrimitivesPerInstance);
    }

    public void Delete() {
        vao.Delete();
        positionVBO.Delete();
        texture.Delete();
    }
}
