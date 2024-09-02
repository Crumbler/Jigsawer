#version 430

layout(location = 0) in vec4 rect;
layout(location = 1) in vec4 color;
layout(location = 2) in vec4 hoverColor;
layout(location = 3) in float hoverFactor;

layout(location = 0) uniform mat3 projMat;

out flat vec4 fColor;

void main()
{
    fColor = mix(color, hoverColor, hoverFactor);

    vec3 pos;

    float rightOrLeft = gl_VertexID & 1;
    float downOrUp = step(2, gl_VertexID);
    
    pos.x = rect[0] * (1.0 - rightOrLeft) + rect[2] * rightOrLeft;
    pos.y = rect[1] * (1.0 - downOrUp) + rect[3] * downOrUp;
    // Necessary for matrix multiplication
    pos.z = 1;

    pos = projMat * pos;

    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}