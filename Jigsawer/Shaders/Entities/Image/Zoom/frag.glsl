#version 430

layout(location = 0) uniform sampler2D texture0;

in vec2 uv;

out vec4 outColor;

void main()
{
    vec3 col = texture(texture0, uv).rgb;
    outColor = vec4(col, 1.0);
}