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

struct PointLight{
	vec3 position;
	vec3 color;
	float intensity;

	float constant;
	float linear;
	float quadratic;
};

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

#define MAX_DIRECTIONAL_LIGHT_COUNT 5
uniform int directionalLightCount;
uniform DirectionalLight directionalLights[MAX_DIRECTIONAL_LIGHT_COUNT];
#define MAX_POINT_LIGHT_COUNT 30
uniform int pointLightCount;
uniform PointLight pointLights[MAX_POINT_LIGHT_COUNT];
#define MAX_SPOT_LIGHT_COUNT 10
uniform int spotLightCount;
uniform SpotLight spotLights[MAX_SPOT_LIGHT_COUNT];
uniform Material material;
uniform sampler2D texture0;


vec3 DirectionalLightContribution(DirectionalLight light, vec3 viewDirection){
	vec3 lightColor = light.color * light.intensity;
	vec3 lightDirection = normalize(-light.direction);

	float diffuseCoefficient = clamp(dot(normal, lightDirection), 0f, 1f);
	vec3 diffuse = diffuseCoefficient * lightColor;

	vec3 reflectionDirection = reflect(-lightDirection, normal);
	float specularCoefficient = pow(max(dot(viewDirection, reflectionDirection), 0f), material.shininess);
	vec3 specular = material.specularStrength * specularCoefficient * lightColor;

	return diffuse + specular;
}

vec3 PointLightContribution(PointLight light){
	vec3 lightColor = light.color * light.intensity;
	float lightDistance = length(light.position - fragmentPosition);
	float attenuation = 1.0 / (light.constant + light.linear * lightDistance + light.quadratic * lightDistance * lightDistance);

	vec3 lightDirection = normalize(light.position - fragmentPosition);
	float diffuseCoefficient = clamp(dot(normal, lightDirection), 0f, 1f);
	vec3 diffuse = diffuseCoefficient * lightColor;

	vec3 viewDirection = normalize(viewPosition - fragmentPosition);
	vec3 reflectionDirection = reflect(-lightDirection, normal);
	float specularCoefficient = pow(max(dot(viewDirection, reflectionDirection), 0f), material.shininess);
	vec3 specular = material.specularStrength * specularCoefficient * lightColor;

	return ( diffuse + specular)  * attenuation;

}

vec3 SpotLightContribution(SpotLight light){
	float cutOff = cos(radians(light.cutOffAngle));
	float outerCutOff = cos(radians(light.cutOffAngle + light.smoothingAngle));

	float lightDistance = length(light.position - fragmentPosition);
	float attenuation = 1.0 / (light.constant + light.linear * lightDistance + light.quadratic * lightDistance * lightDistance);

	vec3 lightDirection = normalize(light.position - fragmentPosition);
	float theta = dot(lightDirection, normalize(-light.direction));
	float epsilon = cutOff - outerCutOff;
	float intensity = clamp((theta - outerCutOff) / epsilon, 0f, 1f);
	vec3 lightColor = normalize(light.color) * intensity;

	vec3 diffuse, specular;
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
	
	return diffuse + specular;
}




void main(){
	vec3 objectColor = vec3(texture(texture0, texCoord));
	vec3 viewDirection = normalize(viewPosition - fragmentPosition);

	vec3 ambient = material.ambientStrength * material.ambientColor;

	vec3 directionalLightComponent = vec3(0f,0f,0f);
	for(int i=0; i<directionalLightCount; i++) {
		directionalLightComponent += DirectionalLightContribution(directionalLights[i], viewDirection);
	}

	vec3 pointLightComponent = vec3(0f, 0f, 0f);
	for(int i=0; i<pointLightCount; i++){
		pointLightComponent += PointLightContribution(pointLights[i]);
	}

	vec3 spotLightComponent = vec3(0f, 0f, 0f);
	for(int i=0; i<spotLightCount; i++){
		spotLightComponent += SpotLightContribution(spotLights[i]);
	}

	vec3 result = (ambient + directionalLightComponent + pointLightComponent + spotLightComponent) * objectColor;

	FragColor = vec4(result, 1.0f);
}
