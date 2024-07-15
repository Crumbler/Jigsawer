#version 330 core

layout(location = 0) in vec4 box;

void main()
{
    vec2 pos;
    
    pos.x = box[0] + box[2] * (gl_VertexID / 2);
    pos.y = box[1] + box[3] * step(2, (gl_VertexID + 1) % 4);


    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}