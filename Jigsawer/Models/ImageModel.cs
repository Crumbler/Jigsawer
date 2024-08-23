﻿
using Jigsawer.Main;
using static Jigsawer.Shaders.Programs.ImageShaderProgram;

using OpenTK.Graphics.OpenGL4;
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

            positionVBO.Orphan();
            positionVBO.SetData(InstanceDataSize, value);
        }
    }

    public ImageModel(ref Matrix3 projMat, float textureSizeMultiplier) {
        vao = VAO.Create();

        positionVBO = VBO.Create(BufferUsageHint.StaticDraw);
        positionVBO.SetData(InstanceDataSize, box);

        vao.EnableVertexAttributeArray(AttributePositions.Position);
        vao.BindAttributeToPoint(AttributePositions.Position, 0);
        vao.SetBindingPointToBuffer(0, positionVBO.Id);
        // 1 attribute value per instance
        vao.SetBindingPointDivisor(0, 1);
        vao.SetAttributeFormat(AttributePositions.Position, PrimitivesPerInstance, VertexAttribType.Float);

        texture = Texture.Create(TextureUnit.Texture0, Images.Image.MainMenuBackgroundTile);
        Texture.SetMinFilter(TextureMinFilter.Linear);
        Texture.SetMagFilter(TextureMagFilter.Linear);
        Texture.SetWrapping(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat);
        Texture.SetWrapping(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat);

        imageShader = Create();
        imageShader.Use();
        SetProjectionMatrix(ref projMat);
        SetTextureSize(texture.Size * textureSizeMultiplier);
        SetTextureUnit(0);
    }

    public void UpdateProjectionMatrix(ref Matrix3 mat) {
        imageShader.Use();
        SetProjectionMatrix(ref mat);
    }

    public void Render() {
        vao.Bind();

        texture.Activate();
        texture.Bind();

        imageShader.Use();

        GL.DrawArrays(PrimitiveType.TriangleFan, 0, PrimitivesPerInstance);
    }

    public void Delete() {
        vao.Delete();
        positionVBO.Delete();
        texture.Delete();
    }
}
