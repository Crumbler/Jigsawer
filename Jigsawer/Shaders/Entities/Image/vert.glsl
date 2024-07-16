#version 330 core

layout(location = 0) in vec4 box;

uniform mat3 projMat;

out vec2 uv;

void main()
{
    vec3 pos;
    
    pos.x = box[0] + box[2] * (gl_VertexID / 2);
    pos.y = box[1] + box[3] * step(2, (gl_VertexID + 1) % 4);
    pos.z = 1;

    uv = pos.xy;

    pos = projMat * pos;

    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}