#version 430

layout(location = 0) uniform sampler2D texture0;

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

    vec2 texSize = textureSize(texture0, 0);
    vec2 blockSize = box.zw - box.xy;

    float texRatio = texSize.x / texSize.y;

    vec2 widthBasedSize = vec2(blockSize.x, blockSize.x / texRatio);
    vec2 heightBasedSize = vec2(blockSize.y * texRatio, blockSize.y);

    vec2 endSize = min(widthBasedSize, heightBasedSize);

    vec3 pos;
    
    //pos.x = mix(box[0], box[2], isRight);
    //pos.y = mix(box[1], box[3], isDown);
    pos.x = (box[0] + box[2]) / 2.0 + (isRight * 2.0 - 1.0) * endSize.x / 2.0;
    pos.y = (box[1] + box[3]) / 2.0 + (isDown * 2.0 - 1.0) * endSize.y / 2.0;
    // Necessary for matrix multiplication
    pos.z = 1;

    //uv = pos.xy - box.xy;
    uv = vec2(isRight, isDown);

    pos = projMat * pos;

    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}