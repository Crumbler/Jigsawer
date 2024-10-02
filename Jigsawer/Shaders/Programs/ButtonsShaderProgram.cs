﻿
using Jigsawer.Scenes;

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Shaders.Programs;

public class ButtonsShaderProgram : ShaderProgram {
    private const string EntityName = "Buttons";

    public ButtonsShaderProgram() {
        Initialize(
            ShaderInfo.Get(EntityName, ShaderType.VertexShader),
            ShaderInfo.Get(EntityName, ShaderType.FragmentShader));

        ConnectUniformBlockToBuffer(UniformBlockNames.SharedInfo, Globals.SharedInfoBindingPoint);
    }

    public static class AttributePositions {
        public const int Rectangle = 0,
            Color = 1,
            HoverColor = 2,
            HoverFactor = 3;
    }

    private static class UniformBlockNames {
        public const string SharedInfo = "SharedInfo";
    }
}
