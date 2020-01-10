#version 330 core
out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;
in vec3 fragmentPosition;

uniform vec3 viewPosition;


struct SpotLight{
	vec3 position;
	vec3 direction;
	float cutOffAngle;
	float smoothingAngle;
	vec3 color;
	float intensity;

	float constant;
	float linear;
	float quadratic;
};

struct Material{
	vec3 ambientColor;
	float ambientStrength;
	float specularStrength;
	float shininess;
};

uniform SpotLight light;
uniform Material material;


uniform sampler2D texture0;


void main(){
	float cutOff = cos(radians(light.cutOffAngle));
	float outerCutOff = cos(radians(light.cutOffAngle + light.smoothingAngle));

	float lightDistance = length(light.position - fragmentPosition);
	float attenuation = 1.0 / (light.constant + light.linear * lightDistance + light.quadratic * lightDistance * lightDistance);

	vec3 lightDirection = normalize(light.position - fragmentPosition);
	float theta = dot(lightDirection, normalize(-light.direction));
	float epsilon = cutOff - outerCutOff;
	float intensity = clamp((theta - outerCutOff) / epsilon, 0f, 1f);

	vec3 objectColor = vec3(texture(texture0, texCoord));
	vec3 lightColor = normalize(light.color) * intensity;

	vec3 ambient, diffuse, specular;
	ambient = material.ambientStrength * material.ambientColor;
	if(theta > outerCutOff){
		float diffuseCoefficient = clamp(dot(normal, lightDirection), 0f, 1f);
		diffuse = diffuseCoefficient * lightColor * attenuation;

		vec3 viewDirection = normalize(viewPosition - fragmentPosition);
		vec3 reflectionDirection = reflect(-lightDirection, normal);
		float specularCoefficient = pow(max(dot(viewDirection, reflectionDirection), 0f), material.shininess);
		specular = material.specularStrength * specularCoefficient * lightColor * attenuation;
	}else{
		diffuse = vec3(0f, 0f, 0f);
		specular = vec3(0f, 0f, 0f);
	}
	
	vec3 result = (ambient + diffuse + specular) * objectColor;

	FragColor = vec4(result, 1.0f);
}
