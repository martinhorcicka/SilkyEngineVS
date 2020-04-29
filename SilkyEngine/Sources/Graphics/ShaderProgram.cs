using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;
using SilkyEngine.Sources.Graphics.Structs;
using SilkyEngine.Sources.Interfaces;

namespace SilkyEngine.Sources.Graphics
{
    public abstract class ShaderProgram : IDisposable, IBindable
    {
        protected GL gl;
        uint id;
        private Dictionary<string, int> uniformLocations = new Dictionary<string, int>();

        protected ShaderProgram(GL gl, string name)
        {
            this.gl = gl;
            id = CreateProgram(name);
        }

        public virtual void Bind()
        {
            gl.UseProgram(id);
        }

        public virtual void Unbind()
        {
            gl.UseProgram(0);
        }

        protected unsafe void SetUniform(string name, Matrix4x4 matrix)
        {
            gl.UniformMatrix4(GetUniformLocation(name), 1, false, (float*)&matrix);
        }

        protected void SetUniform(string name, Vector3 vector)
        {
            gl.Uniform3(GetUniformLocation(name), ref vector);
        }

        protected void SetUniform(string name, int value)
        {
            gl.Uniform1(GetUniformLocation(name), value);
        }

        protected void SetUniform(string name, float value)
        {
            gl.Uniform1(GetUniformLocation(name), value);
        }

        protected void SetUniform(string name, LightStruct light)
        {
            SetUniform(name + ".position", light.position);

            SetUniform(name + ".diffuse", light.diffuse);
            SetUniform(name + ".ambient", light.ambient);
            SetUniform(name + ".specular", light.specular);

            SetUniform(name + ".constant", light.constant);
            SetUniform(name + ".linear", light.linear);
            SetUniform(name + ".quadratic", light.quadratic);
        }

        protected int GetUniformLocation(string name)
        {
            if (uniformLocations.ContainsKey(name))
                return uniformLocations[name];

            int location = gl.GetUniformLocation(id, name);
            uniformLocations.Add(name, location);
            return location;
        }

        private uint CreateProgram(string name)
        {
            uint program = gl.CreateProgram();
            uint vertexShader = CreateShader(name, ShaderType.VertexShader);
            uint fragmentShader = CreateShader(name, ShaderType.FragmentShader);
            gl.AttachShader(program, vertexShader);
            gl.AttachShader(program, fragmentShader);
            gl.LinkProgram(program);
            string infoLog = gl.GetProgramInfoLog(program);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception("Failed to link shader program:\n" + infoLog);
            }

            gl.DetachShader(program, vertexShader);
            gl.DeleteShader(vertexShader);
            gl.DetachShader(program, fragmentShader);
            gl.DeleteShader(fragmentShader);

            return program;
        }

        private uint CreateShader(string name, ShaderType type)
        {
            uint shader = gl.CreateShader(type);
            bool isVertexShader = (type == ShaderType.VertexShader);
            string shaderSource = File.ReadAllText("Resources/Shaders/" + name + (isVertexShader ? ".vert" : ".frag"));
            gl.ShaderSource(shader, shaderSource);
            gl.CompileShader(shader);
            string infoLog = gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception($"Failed compilation of the {(isVertexShader ? "VERTEX" : "FRAGMENT")} shader:\n{infoLog}");
            }

            return shader;
        }

        public void Dispose()
        {
            gl.DeleteProgram(id);
        }
    }
}