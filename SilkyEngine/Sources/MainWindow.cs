using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Controls;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Graphics.Structs;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Physics;
using SilkyEngine.Sources.Tools;

namespace SilkyEngine.Sources
{
    public class MainWindow
    {
        private IWindow window;
        private GL gl;
        private Renderer renderer;
        private Loader loader;
        private Shader basicShader, terrainShader, lightShader;
        private World world;

        public MainWindow() : this(DefaultWindowOptions()) { }
        public MainWindow(WindowOptions options)
        {
            window = Window.Create(options);
            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Update += CollisionDetection.CheckForCollisions;
            window.Closing += OnClose;
        }

        public void Run()
        {
            window.Run();
        }

        private void OnLoad()
        {
            Func<float, float> sin = MathF.Sin, cos = MathF.Cos;
            Func<float, float, float> CustomHeightMap = (x, y) =>
            {
                x /= 1; y /= 1;
                x -= 4;
                float f = sin(cos(x)) * sin(cos(y));
                return f;
            };

            var input = window.CreateInput();
            foreach (var kb in input.Keyboards)
            {
                kb.KeyDown += OnKeyDown;
                kb.KeyUp += OnKeyUp;
            }
            gl = GL.GetApi();
            loader = new Loader(gl);
            var controls = new ThirdPersonControls(window);
            CollisionDetection.Collision += controls.OnCollision;
            world = new World(window, loader, controls);
            renderer = new Renderer(gl, window, controls);

            basicShader = loader.LoadShader("basic");
            terrainShader = loader.LoadShader("basicTerrain");
            terrainShader.SubscribeUniform("texScale", 2f);
            lightShader = loader.LoadShader("simpleLight");

            renderer.SubscribeShader(ShaderTypes.Basic, basicShader);
            renderer.SubscribeShader(ShaderTypes.Terrain, terrainShader);
            renderer.SubscribeShader(ShaderTypes.Light, lightShader);

            world.Finish(renderer);

            renderer.Prepare();
        }

        private void OnRender(double deltaTime)
        {
            renderer.Draw();
        }

        private void OnUpdate(double deltaTime)
        {
            window.Title = $"FPS: {1 / deltaTime:0}";
        }

        private void OnKeyDown(IKeyboard keyboard, Key key, int code)
        {
            switch (key)
            {
                case Key.Escape:
                    window.Close();
                    break;
            }
        }

        private void OnKeyUp(IKeyboard keyboard, Key key, int code)
        {
            switch (key)
            {
                case Key.F11:
                    window.WindowState = (window.WindowState == WindowState.Normal) ? WindowState.Fullscreen : WindowState.Normal;
                    break;
                case Key.M:
                    window.CreateInput().Mice[0].Cursor.CursorMode = (window.CreateInput().Mice[0].Cursor.CursorMode == CursorMode.Disabled) ? CursorMode.Normal : CursorMode.Disabled;
                    break;
                case Key.V:
                    window.VSync = (window.VSync == VSyncMode.Off) ? VSyncMode.On : VSyncMode.Off;
                    break;
                case Key.P:
                    gl.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Fill);
                    break;
                case Key.L:
                    gl.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Line);
                    break;
            }
        }

        private void OnClose()
        {
            System.Console.WriteLine("Closing..");
            loader.Dispose();
        }

        private static WindowOptions DefaultWindowOptions()
        {
            GraphicsAPI API = GraphicsAPI.Default;
            API.API = ContextAPI.OpenGL;
            API.Version = new APIVersion(3, 3);
            API.Profile = ContextProfile.Core;
            API.Flags = ContextFlags.ForwardCompatible;

            var options = WindowOptions.Default;
            options.API = API;
            options.WindowBorder = WindowBorder.Fixed;
            options.Size = new Size(1280, 720);
            options.PreferredDepthBufferBits = 24;

            return options;
        }
    }
}