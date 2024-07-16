#version 330 core

uniform vec2 textureSize;

uniform sampler2D texture0;

in vec2 uv;

out vec4 outColor;

void main()
{
    
    vec3 col = texture(texture0, uv / textureSize).rgb;
    outColor = vec4(col, 1.0);
}