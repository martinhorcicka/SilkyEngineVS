#version 330 core

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

in vec3 fPosition;
in vec2 fTexCoords;
in vec3 fNormal;

out vec4 FragColor;

uniform vec3 viewPos; 
uniform sampler2D colorTexture;
uniform Light light, light2, light3;

vec3 computeLighting(Light light) {
    // ambient
    vec3 ambient = light.ambient;
    //diffuse
    vec3 norm = normalize(fNormal);
    vec3 lightDir = normalize(light.position - fPosition);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * light.diffuse;
    // specular
    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - fPosition);
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * light.specular;  
    // attenuation
    float distance = length(light.position - fPosition);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
                                light.quadratic * (distance * distance));

    return (ambient + diffuse + specular) * attenuation;
}

vec3 clampVec3(vec3 vec) {
    if (vec.x > 1) vec.x = 1;
    if (vec.y > 1) vec.y = 1;
    if (vec.z > 1) vec.z = 1;

    return vec;
}

void main() {    
    vec3 lighting = vec3(0);
    lighting += computeLighting(light);
    lighting += computeLighting(light2);
    lighting += computeLighting(light3);
    lighting = clampVec3(lighting);
    vec3 objectColor = texture(colorTexture, fTexCoords).xyz;
    FragColor = vec4(lighting * objectColor, 1.0);
}