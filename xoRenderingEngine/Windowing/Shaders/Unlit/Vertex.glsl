#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 inTexCoord;
layout (location = 2) in vec3 inNormal;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(){
	texCoord = vec2(inTexCoord.x, inTexCoord.y);
	gl_Position = projection * view * model * vec4(position, 1.0f);
}