#version 330 core
layout (location = 0) in vec3 position;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform vec4 inColor;

out vec4 color;

void main(){
	color = inColor;
	gl_Position = projection * view * model * vec4(position, 1.0f);
	gl_Position.z -= 0.00001f;
}