#version 430

layout(location = 0) in vec4 box;

layout (std140) uniform SharedInfo
{
    int time;
    mat3 projMat;
};

out vec2 uv;

void main()
{
    float isRight = gl_VertexID & 1;
    float isDown = step(2, gl_VertexID);

    vec3 pos;
    
    pos.x = mix(box[0], box[2], isRight);
    pos.y = mix(box[1], box[3], isDown);
    // Necessary for matrix multiplication
    pos.z = 1;

    uv = pos.xy - box.xy;

    pos = projMat * pos;

    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}