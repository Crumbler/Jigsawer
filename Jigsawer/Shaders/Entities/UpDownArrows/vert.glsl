#version 430

layout(location = 0) in vec4 rect;
layout(location = 1) in int isArrowDown;
layout(location = 2) in vec4 color;

layout (std140) uniform SharedInfo
{
    int time;
    mat3 projMat;
};

out flat vec4 fColor;

void main()
{
    fColor = color;

    vec3 pos;

    int vId = gl_VertexID;

    float width = rect[2] - rect[0];

    float rightOrLeft = vId & 1;
    //float downOrUp = vId == 1 ? 0.0 : 1.0;
    float downOrUp = 1 - abs(step(2, (vId + 1) % 3) - isArrowDown);
    
    pos.x = rect[0] + width * vId / 2.0;
    pos.y = rect[1] * (1.0 - downOrUp) + rect[3] * downOrUp;
    // Necessary for matrix multiplication
    pos.z = 1;

    pos = projMat * pos;

    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}