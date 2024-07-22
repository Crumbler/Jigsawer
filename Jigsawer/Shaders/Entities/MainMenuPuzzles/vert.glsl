#version 330 core

uniform int time;
uniform vec2 drawSize;

out vec2 uv;

const float pi = acos(-1),
    twoPi = pi * 2;

const float puzzleR = 0.3,
    circleR = puzzleR * 2.0;

const float triangleTopY = sqrt(3.0) / 2.0;

void main()
{
    float angle = (gl_VertexID / 3) * twoPi / 4.0 /*+ float(time) / 200*/;
    float localAngle = (gl_VertexID % 3) * twoPi / 3.0;

    vec2 ratioFix = min(vec2(drawSize.y / drawSize.x, drawSize.x / drawSize.y), 1.0);

    vec2 basePos = vec2(cos(angle), sin(angle)) * ratioFix * (1.0 - puzzleR);

    vec2 pos = basePos + vec2(cos(localAngle), sin(localAngle)) * ratioFix * circleR;

    uv.x = ((gl_VertexID + 1) % 3 - 1.0) / 2.0;
    uv.y = triangleTopY * step(gl_VertexID % 3, 0.0);

    gl_Position = vec4(pos.x, pos.y, 0, 1);
}