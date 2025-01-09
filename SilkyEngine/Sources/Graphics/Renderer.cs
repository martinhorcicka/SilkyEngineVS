using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Tools;

namespace SilkyEngine.Sources.Graphics
{
    public class Renderer
    {
        private GL gl;
        private IWindow window;
        private Camera camera;
        private List<LightEntity> lights = new List<LightEntity>();
        private Dictionary<ShaderTypes, Dictionary<TexturedModel, List<Entity>>> renderables = new Dictionary<ShaderTypes, Dictionary<TexturedModel, List<Entity>>>();
        private Dictionary<ShaderTypes, Shader> shaders = new Dictionary<ShaderTypes, Shader>();
        private Matrix4x4 projectionMatrix;
        public Renderer(GL gl, IWindow window, ICameraController controls)
        {
            this.gl = gl;
            this.window = window;
            camera = new Camera(controls, new Vector3(0, 3, -15), Vector3.Zero);

            window.Resize += OnResize;
        }

        private void MakeProjection() => projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(Computation.DegToRad(45), (float)window.Size.X / window.Size.Y, 0.5f, 200f);
        public void Prepare()
        {
            gl.Enable(EnableCap.DepthTest);
            gl.DepthFunc(DepthFunction.Less);

            gl.Enable(EnableCap.CullFace);
            gl.CullFace(GLEnum.Front);
            gl.FrontFace(FrontFaceDirection.CW);

            gl.ClearColor(Color.LightSkyBlue);
            MakeProjection();
        }

        public void SubscribeShader(ShaderTypes type, Shader shader)
        {
            if (shaders.ContainsKey(type))
                return;

            shaders.Add(type, shader);
            renderables.Add(type, new Dictionary<TexturedModel, List<Entity>>());
        }

        public void SubscribeRenderables<T>(List<T> entities, ShaderTypes shaderType) where T : Entity
        {
            foreach (var entity in entities)
                SubscribeRenderable(entity, shaderType);
        }

        public void SubscribeRenderable(Entity entity, ShaderTypes shaderType)
        {
            if (entity is LightEntity newLight)
            {
                newLight.LightMoved += OnLightMove;
                lights.Add(newLight);
                UpdateLights();
            }
            if (!renderables.ContainsKey(shaderType))
                throw new Exception("Shader of this type is not subscribed!");

            if (renderables[shaderType].Count == 0)
            {
                renderables[shaderType].Add(entity.TexturedModel, new List<Entity>() { entity });
                UpdateLights();
                shaders[shaderType].SubscribeUniform("model", Matrix4x4.Identity);
                shaders[shaderType].SubscribeUniform("itModel", Matrix4x4.Identity);
                shaders[shaderType].SubscribeUniform("view", Matrix4x4.Identity);
                shaders[shaderType].SubscribeUniform("proj", Matrix4x4.Identity);
                shaders[shaderType].SubscribeUniform("viewPos", Vector3.One);
                return;
            }

            if (!renderables[shaderType].ContainsKey(entity.TexturedModel))
            {
                renderables[shaderType].Add(entity.TexturedModel, new List<Entity>() { entity });
                return;
            }

            renderables[shaderType][entity.TexturedModel].Add(entity);
        }

        private void UpdateLights()
        {
            foreach (var item in renderables)
            {
                Shader shader = shaders[item.Key];
                foreach (var light in lights)
                {
                    shader.SubscribeUniform(light.GetLightStructName(), light.GetLightStruct());
                }
            }
        }

        private void OnLightMove()
        {
            foreach (var item in renderables)
            {
                Shader shader = shaders[item.Key];
                foreach (var light in lights)
                {
                    shader.Bind();
                    shader.UpdateUniform(light.GetLightStructName(), light.GetLightStruct());
                }
            }
        }

        public void Draw()
        {
            gl.Clear((uint)(GLEnum.ColorBufferBit | GLEnum.DepthBufferBit));
            foreach (var renderable in renderables)
            {
                Shader shader = shaders[renderable.Key];
                shader.Bind();
                shader.UpdateUniform("proj", projectionMatrix);
                camera.UpdateView(shader);
                foreach (var model in renderable.Value)
                {
                    model.Key.Bind();
                    foreach (var entity in model.Value)
                    {
                        EntityRenderer.Draw(gl, entity, shader);
                    }
                }
            }
        }

        public Shader GetShader(ShaderTypes type)
        {
            if (!shaders.ContainsKey(type))
                throw new Exception("A shader of this type is not subscribed to this renderer!");

            return shaders[type];
        }

        private void OnResize(Vector2D<int> newSize)
        {
            gl.Viewport(newSize);
            MakeProjection();
        }
    }
}