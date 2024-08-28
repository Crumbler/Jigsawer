#version 430

layout(location = 1) uniform float scaleFactor;

layout(location = 2) uniform sampler2D texture0;

in vec2 uv;

out vec4 outColor;

void main()
{
    vec2 texSize = textureSize(texture0, 0);
    vec3 col = texture(texture0, uv / scaleFactor / texSize).rgb;
    outColor = vec4(col, 1.0);
}