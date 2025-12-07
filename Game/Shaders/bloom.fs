#version 330

in vec2 fragTexCoord;
out vec4 finalColor;

uniform sampler2D texture0;

void main() {
    vec4 sum = vec4(0);
    vec2 texelSize = 1.0 / textureSize(texture0, 0);

    for (int x = -2; x <= 2; x++) {
        for (int y = -2; y <= 2; y++) {
            vec2 offset = vec2(x, y) * texelSize;
            vec4 color = texture(texture0, fragTexCoord + offset);
            
            float brightness = dot(color.rgb, vec3(0.2126, 0.7152, 0.0722));
            if (brightness > 0.8) {
                sum += color;
            }
        }
    }
    
    vec4 baseColor = texture(texture0, fragTexCoord);
    finalColor = baseColor + sum * 0.05;
}
