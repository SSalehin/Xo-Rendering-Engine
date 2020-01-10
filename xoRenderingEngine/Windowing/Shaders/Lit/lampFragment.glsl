#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform vec3 color;

void main(){
	FragColor = vec4(color.r, color.g, color.b, 1.0f);
}