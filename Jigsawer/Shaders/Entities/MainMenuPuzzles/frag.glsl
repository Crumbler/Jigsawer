#version 430

// coords x(from -0.5 to 0.5), y(from 0.0 to equilateral triangle height)
in vec2 uv;
flat in int pieceId;

out vec4 outColor;

layout(location = 0) uniform int time;
layout(location = 1) uniform vec2 drawSize;

// innerSide - side of square which fits the puzzle piece
// without outcroppings
const float innerSide = 10.0,
    outRadius = innerSide / 7.0,
    // outerSide - side of square which accounts for puzzle piece
    // with possible outcroppings
    outerSide = innerSide + outRadius * 4.0;
    
const float pi = acos(-1.0);

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
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
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
// anySideMult = 0.0 for edge side, 1.0 for regular side
float getPuzzleSideDist(vec2 coord, float sideMult, float anySideMult) {
    float dist = coord.x - innerSide / 2.0;
    
    float d1 = min(dist, getOutieDist(coord - vec2(innerSide / 2.0, 0.0)));
    float d2 = max(dist, -getOutieDist(vec2(innerSide / 2.0, 0.0) - coord));
    
    float distNonEdge = mix(d2, d1, sideMult);

    dist = mix(dist, distNonEdge, anySideMult);
    
    return dist;
}

float getMult(vec2 coord, out bool isIn) {
    float angle = atan(coord.y, coord.x);
    
    angle = mod(angle + pi * 1.0 / 4.0, pi * 2.0);
    
    float factor = floor(angle / pi * 2.0);
    
    float rotAngle = factor * pi / 2.0;
    
    coord = rotatedVector(coord, rotAngle);

    int pieceInfo = pieceId * 51 + 13;
    
    uint side = (uint(pieceInfo) >> (int(factor) * 2)) & 1u;
    int edge = 1 - (pieceId * int(factor) * 7 / 5) & 1;

    float dist = getPuzzleSideDist(coord, float(side), float(edge));

    isIn = dist <= 0.0;
    
    return step(dist, 0.0);
}

vec3 calculateColor(vec2 coord) {
    float timeFactor = time / 70.0;

    float minSize = min(drawSize.x, drawSize.y);

    vec2 pointA = drawSize / 2.0 - vec2(minSize) * 0.2;
    vec2 pointB = drawSize / 2.0 + vec2(minSize) * 0.2;

    float distA = distance(gl_FragCoord.xy, pointA);
    float distB = distance(gl_FragCoord.xy, pointB);

    float a = abs(sin(cos(distA / 10.0) - timeFactor * 0.2)) * 0.5;
    float b = abs(sin(distB / 10.0 - timeFactor)) * 0.5;

    return vec3(a + b, a - b, b - a);
}

void main()
{
    const float innerR = sqrt(3.0) / 6.0,
        innerSquareSide = innerR / sqrt(2.0);

    // coords (-0.5 to 0.5) in the square that is in the
    // circle inscribed in the triangle
    vec2 coord = (uv - vec2(0.0, innerR)) / innerSquareSide;

    bool isIn;
    float mult = getMult(coord / 2.0 * outerSide, isIn);

    vec3 col = calculateColor(coord / 2.0 * outerSide);

    outColor = vec4(col * mult, 1.0);

    if (!isIn) {
        discard;
    }
}