using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Controls;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Graphics.Structs;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources
{
    public class World
    {
        private Player player;
        private List<Entity> terrain;
        private List<Entity> obstacles;
        private List<LightEntity> lights;
        private HeightMap heightMap;
        Func<float, float, float, Vector3> newPosition;
        private RectangleF walkableArea;
        private bool[,] isWalkable;

        public World(IWindow window, Loader loader)
        {
            Func<float, float> sin = MathF.Sin, cos = MathF.Cos;
            Func<float, float, float> CustomHeightMap = (x, y) =>
            {
                x /= 1; y /= 1;
                x -= 4;
                float f = sin(cos(x)) * sin(cos(y));
                return f;
            };

            walkableArea = new RectangleF(-100, -100, 200, 200);
            isWalkable = new bool[(int)walkableArea.Width, (int)walkableArea.Height];
            for (int i = 0; i < isWalkable.GetLength(0); i++)
            {
                for (int j = 0; j < isWalkable.GetLength(1); j++)
                {
                    isWalkable[i, j] = true;
                }
            }

            heightMap = new HeightMap("ltm_heightmap.png", walkableArea, 0, 20);
            newPosition = (x, y, z) => new Vector3(x, y + heightMap.GetHeight(x, z), z);

            var rotation = new BRotateAroundY(window, 2);
            var counterRotation = new BRotateAroundY(window, -5);
            var rotateArounOrigin = new BRotateAround(window, Vector3.Zero, Vector3.UnitY, 1, heightMap.GetHeight);
            var randomWalking = new BRandomWalk(window, 2, 4, heightMap.GetHeight);
            CreateTerrain(loader);
            CreateObstacles(loader, rotation, counterRotation);
            CreateLights(loader, rotateArounOrigin, randomWalking);

        }

        public bool IsWalkable(Vector3 position)
        {
            float x = position.X, z = position.Z;
            if (!walkableArea.Contains(x, z))
                return false;

            x += walkableArea.Width / 2;
            z += walkableArea.Height / 2;

            return isWalkable[(int)x, (int)z];
        }

        private void DisableWalking(Vector3 position)
        {
            float x = position.X, z = position.Z;
            if (!walkableArea.Contains(x, z))
                return;

            x += walkableArea.Width / 2;
            z += walkableArea.Height / 2;

            isWalkable[(int)x, (int)z] = false;
        }

        public float GetHeight(float x, float y) => heightMap.GetHeight(x, y);

        public void MakePlayer(Loader loader, string OBJModel, string texName, string texFormat, IPlayerController controller)
        {
            player = new Player(controller, loader.FromOBJ(OBJModel, texName, texFormat), newPosition(0, 0, 0), Vector3.Zero, 1f);
        }

        private void CreateTerrain(Loader loader)
        {
            terrain = Generator.HeightMapTerrain(loader, "grass", "png", 2f, heightMap.GetHeight);
        }

        private void CreateObstacles(Loader loader, params Behavior[] behaviors)
        {
            obstacles = new List<Entity>()
            {
                new Entity(Behavior.DoNothing ,loader.FromOBJ("cube", "minecraft_stone", "jpg"), newPosition(3.5f,0.5f,0.5f), Vector3.Zero, 0.5f),
                new Entity(behaviors[0],loader.FromOBJ("diamond", "white", "jpg"), newPosition(0.5f,0.5f,3.5f), Vector3.Zero, 0.5f),
                new Entity(behaviors[1],loader.FromOBJ("icosahedron", "yellow", "jpg"), newPosition(0.5f,0.5f,-3.5f), Vector3.Zero, 0.5f),
            };

            foreach (var e in obstacles)
            {
                DisableWalking(e.Position);
            }
        }

        private void CreateLights(Loader loader, params Behavior[] behaviors)
        {
            var lightModel = loader.FromOBJ("sphere", "white", "jpg");

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
                new LightEntity(behaviors[0], light3, lightModel, 0.15f),
                new LightEntity(Behavior.DoNothing, light, lightModel, 10f),
                new LightEntity(behaviors[1], light2, lightModel, 0.2f),
            };
        }

        public void Finish(Renderer renderer)
        {
            renderer.SubscribeRenderable(player, ShaderTypes.Basic);
            renderer.SubscribeRenderables(obstacles, ShaderTypes.Basic);
            renderer.SubscribeRenderables(terrain, ShaderTypes.Terrain);
            renderer.SubscribeRenderables(lights, ShaderTypes.Light);
        }

    }
}
