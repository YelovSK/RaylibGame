#version 330

in vec2 fragTexCoord;
out vec4 finalColor;

uniform sampler2D texture0;

vec3 aces(vec3 x) {
  const float a = 2.51;
  const float b = 0.03;
  const float c = 2.43;
  const float d = 0.59;
  const float e = 0.14;
  return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0);
}

void main() {
    vec4 color = texture(texture0, fragTexCoord);
    vec3 aces = aces(color.rgb);

    finalColor = vec4(aces, 1.0);
}
