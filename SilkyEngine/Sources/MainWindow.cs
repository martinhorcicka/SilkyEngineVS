using System;
using System.Drawing;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Graphics;

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
            window.Closing += OnClose;
        }

        public void Run()
        {
            window.Run();
        }

        private void OnLoad()
        {
            gl = GL.GetApi();

            loader = new Loader(gl);
            world = new World(window, loader);
            renderer = new Renderer(gl, window, world.Controller);

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

        double timeMeasured = 1;
        double remaining = 1;
        int count = 0;
        double avg = 1 / 60d;
        private void OnUpdate(double deltaTime)
        {
            if (remaining < 0)
            {
                avg /= count;
                count = 0;
                remaining = timeMeasured;
                window.Title = $"FPS: {1 / avg:0}";
                avg = 0;
            }
            else
            {
                count++;
                avg += deltaTime;
                remaining -= deltaTime;
            }
        }

        private void OnClose()
        {
            window.Load -= OnLoad;
            window.Render -= OnRender;
            window.Update -= OnUpdate;
            window.Closing -= OnClose;
            Console.WriteLine("Closing..");
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
            options.VSync = VSyncMode.Off;
            options.UpdatesPerSecond = 150;

            return options;
        }
    }
}