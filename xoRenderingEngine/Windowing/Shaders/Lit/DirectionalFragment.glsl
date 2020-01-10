#version 330 core
out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;
in vec3 fragmentPosition;

uniform vec3 viewPosition;


struct DirectionalLight{
	vec3 direction;
	vec3 color;
	float intensity;
};

struct Material{
	vec3 ambientColor;
	float ambientStrength;
	float specularStrength;
	float shininess;
};

uniform DirectionalLight light;
uniform Material material;


uniform sampler2D texture0;


void main(){
	vec3 objectColor = vec3(texture(texture0, texCoord));

	vec3 lightColor = normalize(light.color) * light.intensity;

	vec3 ambient = material.ambientStrength * material.ambientColor;

	vec3 lightDirection = normalize(-light.direction);
	float diffuseCoefficient = clamp(dot(normal, lightDirection), 0f, 1f);
	vec3 diffuse = diffuseCoefficient * lightColor;

	vec3 viewDirection = normalize(viewPosition - fragmentPosition);
	vec3 reflectionDirection = reflect(-lightDirection, normal);
	float specularCoefficient = pow(max(dot(viewDirection, reflectionDirection), 0f), material.shininess);
	vec3 specular = material.specularStrength * specularCoefficient * lightColor;

	vec3 result = (ambient + diffuse + specular) * objectColor;

	FragColor = vec4(result, 1.0f);
}
