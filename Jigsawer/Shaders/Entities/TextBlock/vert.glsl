#version 430

layout(location = 0) in vec2 vPos;
layout(location = 1) in uint charId;
layout(location = 2) in vec4 vColor;
layout(location = 3) in float sizeMult;

layout (std140) uniform SharedInfo
{
    int time;
    mat3 projMat;
};

layout (std140) uniform FontInfo
{
    float fontHeight;
    vec4 characterSizes[47];
};

out vec2 fUv;
flat out vec3 fColor;

void main()
{
    fColor = vColor.rgb;

    int ind = gl_VertexID;

    vec4 vec = characterSizes[charId / 2];
    vec2 charSize = vec2(vec[2 * (charId & 1)], vec[2 * (charId & 1) + 1]);

    vec2 uv;

    uv.x = step(2, ind) * charSize.x;
    uv.y = charId * fontHeight + step(1, ind & 1) * charSize.y;
    
    fUv = uv;

    vec3 pos;
    pos.x = vPos.x + charSize.x * sizeMult * step(2, ind);
    pos.y = vPos.y + charSize.y * sizeMult * float(ind & 1);
    // Necessary for matrix multiplication
    pos.z = 1.0;

    pos = projMat * pos;

    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}