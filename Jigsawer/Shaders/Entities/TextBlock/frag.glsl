#version 430

layout(location = 1) uniform sampler2D fontAtlasTexture;

layout (std140, binding = 1) uniform FontData
{
    float fontHeight;
    float characterWidths[94];
};

in vec2 fUv;
flat in vec3 fColor;

out vec4 outColor;

void main()
{
    vec2 uv = fUv;
    vec2 textureSz = textureSize(fontAtlasTexture, 0);
    float texVal = 1.0 - texture(fontAtlasTexture, uv / textureSz).r;
    outColor = vec4(fColor, texVal);
}