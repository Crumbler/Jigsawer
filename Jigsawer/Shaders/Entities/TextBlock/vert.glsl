#version 430

layout(location = 0) in vec2 vPos;
layout(location = 1) in uint charId;

layout(location = 0) uniform mat3 projMat;

layout (std140, binding = 1) uniform FontData
{
    float fontHeight;
    float characterWidths[94];
};

out vec2 fUv;

void main()
{
    int ind = gl_VertexID;

    vec2 uv;

    uv.x = step(2, ind) * characterWidths[ind];
    uv.y = (charId + step(1, ind & 1)) * fontHeight;
    
    fUv = uv;

    vec3 pos;
    pos.x = vPos.x + characterWidths[ind] * step(2, ind);
    pos.y = vPos.y + fontHeight * step(1, ind & 1);
    // Necessary for matrix multiplication
    pos.z = 1.0;

    pos = projMat * pos;

    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}