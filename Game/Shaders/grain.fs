// ShaderToy by luluco250
// https://www.shadertoy.com/view/4t2fRz 
#version 330

in vec2 fragTexCoord;
out vec4 finalColor;

uniform sampler2D texture0;
uniform float iTime;

#define BLEND_MODE 2
#define SPEED 3.0
#define INTENSITY 0.1
#define MEAN 0.0
#define VARIANCE 0.5

vec3 channel_mix(vec3 a, vec3 b, vec3 w) {
    return vec3(mix(a.r, b.r, w.r), mix(a.g, b.g, w.g), mix(a.b, b.b, w.b));
}

float gaussian(float z, float u, float o) {
    return (1.0 / (o * sqrt(2.0 * 3.1415))) * exp(-(((z - u) * (z - u)) / (2.0 * (o * o))));
}

vec3 madd(vec3 a, vec3 b, float w) {
    return a + a * b * w;
}

vec3 screen(vec3 a, vec3 b, float w) {
    return mix(a, vec3(1.0) - (vec3(1.0) - a) * (vec3(1.0) - b), w);
}

vec3 overlay(vec3 a, vec3 b, float w) {
    return mix(a, channel_mix(
        2.0 * a * b,
        vec3(1.0) - 2.0 * (vec3(1.0) - a) * (vec3(1.0) - b),
        step(vec3(0.5), a)
    ), w);
}

float rand(vec2 uv, float t) {
    float seed = dot(uv, vec2(12.9898, 78.233));
    return fract(sin(seed) * 43758.5453 + t);
}

void main() {
    ivec2 iResolution = textureSize(texture0, 0);
    vec2 uv = fragTexCoord * iResolution;
    vec4 color = texture(texture0, fragTexCoord);

    float t = iTime * float(SPEED);
    float noise = rand(uv, t);
    noise = gaussian(noise, float(MEAN), float(VARIANCE * VARIANCE));

    float w = float(INTENSITY);
    vec3 grain = vec3(noise) * (1.0 - color.rgb);

    color.rgb = overlay(color.rgb, grain, w);

    finalColor = color;
}
