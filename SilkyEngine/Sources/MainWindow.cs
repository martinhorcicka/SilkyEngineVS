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
        private List<Entity> basicEntities;
        private LightEntity testLight;
        private Player player;
        private Controller freeCam, thirdPerson;
        private HeightMap heightMap;

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

            basicShader = loader.LoadShader("basic");
            terrainShader = loader.LoadShader("basicTerrain");
            terrainShader.SubscribeUniform("texScale", 2f);
            lightShader = loader.LoadShader("simpleLight");

            // freeCam = new FreeCameraControls(window);
            thirdPerson = new ThirdPersonControls(window);

            heightMap = new HeightMap("ltm_heightmap.png", new RectangleF(-100, -100, 200, 200), 0, 20);
            Func<float, float, float> tmpGetHeight = heightMap.GetHeight;

            Func<float, float, float, Vector3> newPosition = (x, y, z) => new Vector3(x, y + tmpGetHeight(x, z), z);
            player = new Player((IPlayerController)thirdPerson, loader.FromOBJ("capsule", "blue", "jpg"), newPosition(0, 0, 0), Vector3.Zero, 1f);
            var rotation = new BRotateAroundY(window, 2);
            var counterRotation = new BRotateAroundY(window, -5);

            basicEntities = new List<Entity>()
            {
                player,
                new Entity(Behavior.DoNothing ,loader.FromOBJ("cube", "minecraft_stone", "jpg"), newPosition(3.5f,0.5f,0.5f), Vector3.Zero, 0.5f),
                new Entity(rotation,loader.FromOBJ("diamond", "white", "jpg"), newPosition(0.5f,0.5f,3.5f), Vector3.Zero, 0.5f),
                new Entity(counterRotation,loader.FromOBJ("icosahedron", "yellow", "jpg"), newPosition(0.5f,0.5f,-3.5f), Vector3.Zero, 0.5f),
            };

            ((ThirdPersonControls)thirdPerson).SubscribeHeightMap(tmpGetHeight);

            renderer = new Renderer(gl, window, (ICameraController)thirdPerson, new Vector3(0, 3, -15), player);
            renderer.SubscribeRenderables(basicEntities, basicShader);
            renderer.SubscribeRenderables(Generator.HeightMapTerrain(loader, "grass", "png", 2, tmpGetHeight), terrainShader);

            LightStruct light = new LightStruct(
                name: "light",
                position: newPosition(2, 200, 2),
                ambient: new Vector3(0.2f),
                diffuse: new Vector3(0.9f, 0.9f, 0.9f),
                specular: new Vector3(1.0f, 1.0f, 1.0f),
                constant: 1.0f, linear: 0.0014f, quadratic: 0.000007f
            );
            LightStruct light2 = new LightStruct(
                name: "light2",
                position: newPosition(-4, 1.5f, -5),
                ambient: new Vector3(0.2f),
                diffuse: new Vector3(0.9f, 0.9f, 0.9f),
                specular: new Vector3(1.0f, 1.0f, 1.0f),
                constant: 1.0f, linear: 0.35f, quadratic: 0.44f
            );
            LightStruct light3 = new LightStruct(
                name: "light3",
                position: newPosition(-10, 3.5f, 0),
                ambient: new Vector3(0.2f),
                diffuse: new Vector3(0.9f, 0.9f, 0.9f),
                specular: new Vector3(1.0f, 1.0f, 1.0f),
                constant: 1.0f, linear: 0.35f, quadratic: 0.44f
            );

            var rotateArounOrigin = new BRotateAround(window, Vector3.Zero, Vector3.UnitY, 1, tmpGetHeight);
            var randomWalking = new BRandomWalk(window, 2, 4, tmpGetHeight);
            var lightModel = loader.FromOBJ("sphere", "white", "jpg");
            testLight = new LightEntity(rotateArounOrigin, light3, lightModel, 0.15f);
            var lightEntities = new List<LightEntity>()
            {
                testLight,
                new LightEntity(Behavior.DoNothing, light, lightModel, 10f),
                new LightEntity(randomWalking, light2, lightModel, 0.2f),
            };

            renderer.SubscribeRenderables(lightEntities, lightShader);

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