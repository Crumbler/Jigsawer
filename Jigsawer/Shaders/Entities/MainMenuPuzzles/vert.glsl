#version 430

layout(location = 0) uniform int time;
layout(location = 1) uniform vec2 drawSize;

out vec2 uv;
flat out int pieceId;

const float pi = acos(-1),
    twoPi = pi * 2;

const float puzzleR = 0.4,
    circleR = puzzleR * 2.0;

const float triangleTopY = sqrt(3.0) / 2.0;

void main()
{
    float angle = (gl_VertexID / 3) * twoPi / 4.0 + sin(float(time) / 600.0) * 2.0;
    float localAngle = (gl_VertexID % 3) * twoPi / 3.0;

    vec2 ratioFix = min(vec2(drawSize.y / drawSize.x, drawSize.x / drawSize.y), 1.0);

    vec2 basePos = vec2(cos(angle), sin(angle)) * ratioFix * (1.0 - puzzleR - sin(float(time) / 300.0) * 0.1);

    vec2 pos = basePos + vec2(cos(localAngle), sin(localAngle)) * ratioFix * circleR;

    // coords x(from -0.5 to 0.5), y(from 0.0 to equilateral triangle height)
    uv.x = ((gl_VertexID + 1) % 3 - 1.0) / 2.0;
    uv.y = triangleTopY * step(gl_VertexID % 3, 0.0);

    pieceId = gl_VertexID / 3;
    gl_Position = vec4(pos.x, pos.y, 0, 1);
}