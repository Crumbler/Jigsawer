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
        var vbo = new VBO(InstanceDataSize);
        vbo.SetData(box);

        return vbo;
    }

    private VAO InitializeVAO() {
        var imageVao = new VAO();
        imageVao.SetBindingPointToBuffer(0, positionVBO.Id);
        imageVao.SetBindingPointDivisor(0, 1);

        imageVao.EnableVertexAttributeArray(AttributePositions.Position);
        imageVao.BindAttributeToPoint(AttributePositions.Position, 0);
        imageVao.SetAttributeFormat(AttributePositions.Position,
            PrimitivesPerInstance, VertexAttribType.Float);

        return imageVao;
    }

    private ImageShaderProgram InitializeShader(float scaleFactor,
        ImageSizeMode sizeMode) {
        ImageShaderProgram imageShader = sizeMode switch {
            ImageSizeMode.Normal => new NormalImageShaderProgram(),
            _ => new ZoomImageShaderProgram()
        };

        imageShader.SetScaleFactor(scaleFactor);
        imageShader.SetTextureUnit(texture.Unit);

        return imageShader;
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
