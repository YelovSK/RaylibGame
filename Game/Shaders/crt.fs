#version 330

in vec2 fragTexCoord;
out vec4 finalColor;

uniform sampler2D texture0;

void main() {
    ivec2 texSize = textureSize(texture0, 0);
    float width  = float(texSize.x);
    float height = float(texSize.y);

    // Map to -1..1
    vec2 uv = fragTexCoord * 2.0 - 1.0;

    // Barrel distortion
    float k = 0.05;
    uv *= 1.0 + k * dot(uv, uv);

    // Back to 0..1
    uv = uv * 0.5 + 0.5;

    // If outside texture, output black
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) {
        finalColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    // Sample texture normally
    vec3 color = texture(texture0, uv).rgb;

    // Scanlines
    float scan = 0.8 + 0.2 * sin(uv.y * float(textureSize(texture0, 0).y) * 3.1415);
    //color *= scan;

    // Vignette
    float dist = length(uv - 0.5);
    color *= 1.0 - 0.5 * dist;

    finalColor = vec4(color, 1.0);
}