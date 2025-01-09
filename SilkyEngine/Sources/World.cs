using Silk.NET.Windowing;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Controls;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Graphics.Structs;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Physics.Collisions;
using SilkyEngine.Sources.Tools;
using SilkyEngine.Sources.Zones;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SilkyEngine.Sources
{
    public class World
    {
        private Player player;
        private Controller controller;
        private List<ActiveZone> activeZones;
        private List<Terrain> terrain;
        private List<Obstacle> obstacles;
        private List<Movable> movables;
        private List<LightEntity> lights;
        private HeightMap heightMap;
        Func<float, float, float, Vector3> newPosition;
        private RectangleF walkableArea;

        private event Action MoveEntities;

        protected float Gravity { get; } = 20f;

        public World(IWindow window, Loader loader)
        {
            window.Update += OnUpdate;

            controller = new ThirdPersonControls(window);

            walkableArea = new RectangleF(-100, -100, 200, 200);

            heightMap = new HeightMap("trebusin_area.png", walkableArea, 0, 50);
            newPosition = (x, y, z) => new Vector3(x, y + GetHeight(x, z), z);

            activeZones = new List<ActiveZone>() { new HotZone(new RectangleF(-10, -10, 20, 20)) };

            var rotation = new BRotateAroundY(window, speed: 2);
            var counterRotation = new BRotateAroundY(window, speed: -5);
            var rotateArounOrigin = new BRotateAround(window, point: Vector3.Zero, axis: Vector3.UnitY, speed: 1);
            var randomWalkLight = new BRandomWalk(window, sleepTime: 2, walkingSpeed: 4);
            var randomWalkCube = new BRandomWalk(window, sleepTime: 2, walkingSpeed: 5);
            var walkBackAndForth = new BWalkBackAndForth(window, speed: 2, offset: 4 * Vector3.UnitX);

            CreateTerrain(loader);
            CreateMovables(loader);
            CreatePlayer(loader);
            CreateObstacles(loader, rotation, counterRotation, walkBackAndForth, randomWalkCube);
            CreateLights(loader, rotateArounOrigin, randomWalkLight);

            CollisionDetection.AddTerrain(terrain);

            movables[0].Mass = 3f;

            lights.ForEach((light) => { light.GravityOn = false; MoveEntities += light.OnMove; });
            movables.ForEach((movable) => MoveEntities += movable.OnMove);
            MoveEntities += obstacles[^1].OnMove;
        }

        private void OnUpdate(double deltaTime)
        {
            controller.OnUpdate(deltaTime);

            foreach (var m in movables)
            {
                if (m.GravityOn)
                {
                    m.VerticalSpeed -= Gravity * (float)deltaTime;
                    m.DeltaPosition += m.VerticalSpeed * Vector3.UnitY * (float)deltaTime;
                }

                float nextHeight = GetHeight(m.Position + m.DeltaPosition);
                if (nextHeight > m.Position.Y)
                    m.DeltaPosition += (nextHeight - m.Position.Y) * Vector3.UnitY;
            }

            CollisionDetection.CheckCollisions();

            MoveEntities?.Invoke();

            obstacles[^1].DeltaPosition = Vector3.Zero;
            movables.ForEach((m) => m.DeltaPosition = Vector3.Zero);
            lights.ForEach((l) => l.DeltaPosition = Vector3.Zero);
        }

        public bool IsWalkable(Vector3 position)
        {
            float x = position.X, z = position.Z;
            if (!walkableArea.Contains(x, z))
                return false;

            return true;
        }

        public ICameraController Controller => (ICameraController)controller;

        public float GetHeight(Vector3 position) => GetHeight(position.X, position.Z);
        public float GetHeight(float x, float y) => heightMap.GetHeight(x, y);

        private void CreatePlayer(Loader loader)
        {
            player = new Player(this, BoundingBox.Default, (IPlayerController)controller,
                loader.FromOBJ("capsule", "Colors/blue", "jpg"),
                newPosition(0, 0, 0), Vector3.Zero, 1f, new Vector3(1, 2, 1) * 0.5f);

            if (movables == null) movables = new List<Movable>();
            movables.Add(player);
        }

        private void CreateTerrain(Loader loader)
        {
            terrain = Generator.HeightMapTerrain(this, heightMap, loader, "Cartoony/grass", "png", density: 1f);
        }

        private void CreateObstacles(Loader loader, params Behavior[] behaviors)
        {
            obstacles = new List<Obstacle>()
            {
                new Obstacle(this, BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "Cartoony/stone", "jpg"),
                    newPosition(2,0,-3), Vector3.Zero, 1),

                new Obstacle(this, BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "Cartoony/stone", "jpg"),
                    newPosition( 3.5f, 4.5f, 0.5f), Vector3.Zero, 1),

                new Obstacle(this, BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "Cartoony/stone", "jpg"),
                    newPosition( 9.5f, 0.0f,-6.0f), Vector3.Zero, 2),

                new Obstacle(this, BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "Cartoony/stone", "jpg"),
                    newPosition( 7.5f, 0.0f, 9.5f), Vector3.Zero, 3),

                new Obstacle(this, BoundingBox.Default,  behaviors[0], loader.FromOBJ("diamond", "Colors/red", "jpg"),
                    newPosition(-3.5f, 0.0f, 6.5f), Vector3.Zero, 1, new Vector3(1,0.5f,1)),

                new Obstacle(this, BoundingBox.Default, behaviors[2], loader.FromOBJ("platform", "Cartoony/wood", "jpg"),
                    newPosition(7,8,2), Vector3.Zero, 1, new Vector3(1,0.25f,2)),
            };
        }

        private void CreateMovables(Loader loader)
        {
            var newMovables = new List<Movable>() {
                new Movable(this, BoundingSphere.Default, Behavior.DoNothing, loader.FromOBJ("sphere", "Colors/yellow", "jpg"),
                            newPosition(-8.5f, 0.0f, -3.5f), Vector3.Zero, 1),
                new Movable(this, BoundingSphere.Default, Behavior.DoNothing, loader.FromOBJ("sphere", "Colors/red", "jpg"),
                            newPosition(0, 15, 2f), Vector3.Zero, 1),
                new Movable(this, BoundingSphere.Default, Behavior.DoNothing, loader.FromOBJ("sphere", "Colors/blue", "jpg"),
                            newPosition(4f, 20,-3f), Vector3.Zero, 1),
            };

            if (movables == null) movables = new List<Movable>();
            movables.AddRange(newMovables);
        }

        private void CreateLights(Loader loader, params Behavior[] behaviors)
        {
            var lightModel = loader.FromOBJ("sphere", "Colors/white", "jpg");

            var ambient = new Vector3(0.2f);
            var diffuse = new Vector3(0.9f, 0.9f, 0.9f);
            var specular = new Vector3(1.0f, 1.0f, 1.0f);
            LightStruct light = new LightStruct(
                name: "light",
                position: newPosition(2, 200, 2),
                ambient, diffuse, specular,
                constant: 1.0f, linear: 0.0014f, quadratic: 0.000007f
            );
            LightStruct light2 = new LightStruct(
                name: "light2",
                position: newPosition(-4, 1.5f, -5),
                ambient, diffuse, specular,
                constant: 1.0f, linear: 0.35f, quadratic: 0.44f
            );
            LightStruct light3 = light2.NewNameCopy("light3");

            lights = new List<LightEntity>()
            {
                new LightEntity(this, BoundingSphere.None, Behavior.DoNothing, light, lightModel, 10f),
                new LightEntity(this, BoundingSphere.None, behaviors[1], light2, lightModel, 0.2f),
                new LightEntity(this, BoundingSphere.None, behaviors[0], light3, lightModel, 0.15f),
            };
        }

        public void Finish(Renderer renderer)
        {
            renderer.SubscribeRenderable(player, ShaderTypes.Basic);
            renderer.SubscribeRenderables(movables, ShaderTypes.Basic);
            renderer.SubscribeRenderables(obstacles, ShaderTypes.Basic);
            renderer.SubscribeRenderables(terrain, ShaderTypes.Terrain);
            renderer.SubscribeRenderables(lights, ShaderTypes.Light);

            Shader terrainShader = renderer.GetShader(ShaderTypes.Terrain);

            terrainShader.Bind();
            terrainShader.UpdateUniform("texScale", 25f);
        }
    }
}
