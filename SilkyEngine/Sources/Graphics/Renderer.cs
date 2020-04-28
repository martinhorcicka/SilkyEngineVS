using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Interfaces;

namespace SilkyEngine.Sources.Graphics
{
    public class Renderer
    {
        private GL gl;
        private IWindow window;
        private Camera camera;
        private List<LightEntity> lights;
        private Dictionary<Shader, Dictionary<TexturedModel, List<Entity>>> renderables;
        private Matrix4x4 projectionMatrix;
        public Renderer(GL gl, IWindow window, ICameraController controls, Vector3 cameraPos, Player player)
        {
            this.gl = gl;
            this.window = window;
            camera = new Camera(controls, cameraPos, player.Focus);
            renderables = new Dictionary<Shader, Dictionary<TexturedModel, List<Entity>>>();
            lights = new List<LightEntity>();

            window.Resize += OnResize;
        }

        private void MakeProjection() => projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(degToRad(45), (float)window.Size.Width / window.Size.Height, 0.5f, 200f);
        private float degToRad(float deg) => deg * MathF.PI / 180f;
        public void Prepare()
        {
            gl.Enable(EnableCap.DepthTest);
            gl.DepthFunc(DepthFunction.Less);

            gl.Enable(EnableCap.CullFace);
            gl.CullFace(CullFaceMode.Front);
            gl.FrontFace(FrontFaceDirection.CW);

            gl.ClearColor(Color.LightSkyBlue);
            MakeProjection();
        }

        public void SubscribeRenderables<T>(List<T> entities, Shader shader) where T : Entity
        {
            foreach (var entity in entities)
                SubscribeRenderable(entity, shader);
        }

        public void SubscribeRenderable(Entity entity, Shader shader)
        {
            if (entity is LightEntity)
            {
                LightEntity newLight = (LightEntity)entity;
                newLight.LightMoved += OnLightMove;
                lights.Add(newLight);
                UpdateLights();
            }
            if (!renderables.ContainsKey(shader))
            {
                renderables.Add(shader, new Dictionary<TexturedModel, List<Entity>>()
                {
                    { entity.TexturedModel, new List<Entity>() { entity } }
                });
                UpdateLights();
                shader.SubscribeUniform("model", Matrix4x4.Identity);
                shader.SubscribeUniform("view", Matrix4x4.Identity);
                shader.SubscribeUniform("proj", Matrix4x4.Identity);
                shader.SubscribeUniform("viewPos", Vector3.One);
                return;
            }

            if (!renderables[shader].ContainsKey(entity.TexturedModel))
            {
                renderables[shader].Add(entity.TexturedModel, new List<Entity>() { entity });
                return;
            }

            renderables[shader][entity.TexturedModel].Add(entity);
        }

        private void UpdateLights()
        {
            foreach (var item in renderables)
            {
                Shader shader = item.Key;
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
                Shader shader = item.Key;
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
                Shader shader = renderable.Key;
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

        private void OnResize(Size newSize)
        {
            gl.Viewport(newSize);
            MakeProjection();
        }
    }
}