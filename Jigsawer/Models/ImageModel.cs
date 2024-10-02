using static Jigsawer.Shaders.Programs.ImageShaderProgram;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Jigsawer.Shaders.Programs;
using Jigsawer.GLBuffers;
using Jigsawer.GLObjects;

using System.Drawing;

namespace Jigsawer.Models;

public enum ImageSizeMode {
    Normal,
    Zoom
}

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

    public ImageModel(Bitmap bitmap,
        ImageSizeMode sizeMode = ImageSizeMode.Normal,
        float scaleFactor = 1f,
        TextureParameters? textureParameters = null) {
        positionVBO = InitializeVBO();
        vao = InitializeVAO();

        texture = new Texture(bitmap);
        texture.SetParameters(textureParameters ?? Texture.defaultParameters);

        shader = InitializeShader(scaleFactor, sizeMode);
    }

    public ImageModel(string imagePath,
        ImageSizeMode sizeMode,
        float scaleFactor = 1f,
        TextureParameters? textureParameters = null) {
        positionVBO = InitializeVBO();
        vao = InitializeVAO();

        texture = new Texture(imagePath);
        texture.SetParameters(textureParameters ?? Texture.defaultParameters);

        shader = InitializeShader(scaleFactor, sizeMode);
    }

    private VBO InitializeVBO() {
        var positionVBO = new VBO(InstanceDataSize);
        positionVBO.SetData(box);

        return positionVBO;
    }

    private VAO InitializeVAO() {
        var vao = new VAO();
        vao.SetBindingPointToBuffer(0, positionVBO.Id);
        vao.SetBindingPointDivisor(0, 1);

        vao.EnableVertexAttributeArray(AttributePositions.Position);
        vao.BindAttributeToPoint(AttributePositions.Position, 0);
        vao.SetAttributeFormat(AttributePositions.Position,
            PrimitivesPerInstance, VertexAttribType.Float);

        return vao;
    }

    private ImageShaderProgram InitializeShader(float scaleFactor,
        ImageSizeMode sizeMode) {
        ImageShaderProgram shader = sizeMode switch {
            ImageSizeMode.Normal => new NormalImageShaderProgram(),
            _ => new ZoomImageShaderProgram()
        };

        shader.SetScaleFactor(scaleFactor);
        shader.SetTextureUnit(texture.Unit);

        return shader;
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
