#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 inTexCoord;
layout (location = 2) in vec3 inNormal;

out vec2 texCoord;
out vec3 normal;
out vec3 fragmentPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(){
	texCoord = vec2(inTexCoord.x, inTexCoord.y);
	//normal = normalize(inNormal * mat3(transpose(inverse(model))));
	normal = normalize( mat3(model) * inNormal );
	fragmentPosition = vec3(model * vec4(position, 1f));
	gl_Position = projection * view * model * vec4(position, 1.0f);
}