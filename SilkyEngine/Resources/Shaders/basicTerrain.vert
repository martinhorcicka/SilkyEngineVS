#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec3 aNormal;

out vec3 fPosition;
out vec2 fTexCoords;
out vec3 fNormal;

uniform mat4 model;
uniform mat4 itModel;
uniform mat4 view;
uniform mat4 proj;

uniform float texScale;

void main() {
    fPosition = vec3(model * vec4(aPosition, 1.0));
    gl_Position = proj * view * vec4(fPosition, 1);

    fTexCoords = aTexCoords * texScale;

    fNormal = mat3(itModel) * aNormal;
}