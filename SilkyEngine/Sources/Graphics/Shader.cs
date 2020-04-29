using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.OpenGL;
using SilkyEngine.Sources.Graphics.Structs;

namespace SilkyEngine.Sources.Graphics
{
    public class Shader : ShaderProgram
    {
        private Dictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();

        public Shader(GL gl, string name) : base(gl, name)
        {
            SetUniform("colorTexture", 0);
        }

        public void SubscribeUniform(string name, object data)
        {
            if (uniforms.ContainsKey(name))
                return;
            var shaderUniform = new ShaderUniform(data);
            uniforms.Add(name, shaderUniform);
            Bind();
            SetUniform(name);
        }

        public void UpdateUniform(string name, object data)
        {
            if (!uniforms.ContainsKey(name))
                throw new Exception($"Cannot find the uniform \"{name}\"");

            var un = uniforms[name];
            un.UpdateData(data);
            uniforms[name] = un;
            SetUniform(name);
        }

        private void SetUniform(string name) => SetUniform(name, uniforms[name]);

        private void SetUniform(string name, ShaderUniform shaderUniform)
        {
            var data = shaderUniform.GetData();
            if (data is float)
                SetUniform(name, (float)data);
            if (data is int)
                SetUniform(name, (int)data);
            if (data is Vector3)
                SetUniform(name, (Vector3)data);
            if (data is Matrix4x4)
                SetUniform(name, (Matrix4x4)data);
            if (data is LightStruct)
                SetUniform(name, (LightStruct)data);
        }
    }
}