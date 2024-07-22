#version 330 core

in vec2 uv;

out vec4 outColor;

const float innerSide = 10.0,
    outRadius = innerSide / 7.0,
    outerSide = innerSide + outRadius * 4.0;
    
const float pi = acos(-1.0);
    
const uint pieceInfo = 3u;

vec2 rotatedVector(vec2 vec, float angle) {
    float s = sin(-angle),
        c = cos(-angle);
        
    return vec2(vec.x * c - vec.y * s, vec.x * s + vec.y * c);
}

float sdCircle(vec2 p,float r) {
    return length(p) - r;
}

float sdBox(vec2 p, vec2 b) {
    vec2 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp( dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float opSmoothUnion(float d1, float d2, float k)
{
    float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
    return mix(d2, d1, h) - k * h * (1.0 - h);
}

float getOutieDist(vec2 coord) {
    float distCircle = sdCircle(coord - vec2(outRadius, 0.0), outRadius);
    
    float dist = distCircle;
    
    float distSegment = sdSegment(coord, vec2(0, outRadius),
        vec2(0, -outRadius));
        
    dist = opSmoothUnion(distCircle, distSegment, outRadius * 0.5);
        
    return dist;
}

// sideMult = 1.0 for outer, 0.0 for inner
float getPuzzleSideDist(vec2 coord, float sideMult) {
    float dist = coord.x - innerSide / 2.0;
    
    float d1 = min(dist, getOutieDist(coord - vec2(innerSide / 2.0, 0.0)));
    float d2 = max(dist, -getOutieDist(vec2(innerSide / 2.0, 0.0) - coord));
    
    dist = mix(d2, d1, sideMult);
    
    return dist;
}

vec3 getColor(vec2 coord, out bool isIn) {
    float angle = atan(coord.y, coord.x);
    
    angle = mod(angle + pi * 1.0 / 4.0, pi * 2.0);
    
    float factor = floor(angle / pi * 2.0);
    
    float rotAngle = factor * pi / 2.0;
    
    coord = rotatedVector(coord, rotAngle);
    
    uint side = (pieceInfo >> int(factor)) & 1u;

    float dist = getPuzzleSideDist(coord, float(side));

    isIn = dist <= 0.0;
    
    return vec3(step(dist, 0.0));
}

void main()
{
    const float innerR = sqrt(3.0) / 6.0,
        innerSquareSide = innerR / sqrt(2.0);

    vec2 coord = (uv - vec2(0.0, innerR)) / innerSquareSide;

    bool isIn;
    vec3 col = getColor(coord / 2.0 * outerSide, isIn);

    outColor = vec4(col, 1.0);

    if (!isIn) {
        discard;
    }
}