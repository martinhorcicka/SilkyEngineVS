using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Controls;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Graphics.Structs;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Physics;
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

            heightMap = new HeightMap("ltm_heightmap.png", walkableArea, 0, 20);
            newPosition = (x, y, z) => new Vector3(x, y + GetHeight(x, z), z);

            var rotation = new BRotateAroundY(window, 2);
            var counterRotation = new BRotateAroundY(window, -5);
            var rotateArounOrigin = new BRotateAround(window, Vector3.Zero, Vector3.UnitY, 1, GetHeight);
            var randomWalkLight = new BRandomWalk(window, 2, 4, GetHeight);

            var walkBackAndForth = new BWalkBackAndForth(window, 2, 4 * Vector3.UnitX);
            CreateTerrain(loader);
            CreateObstacles(loader, rotation, counterRotation, walkBackAndForth);
            CreateLights(loader, rotateArounOrigin, randomWalkLight);

        }

        public bool IsWalkable(Vector3 position)
        {
            float x = position.X, z = position.Z;
            if (!walkableArea.Contains(x, z))
                return false;

            return true;
        }

        public float GetHeight(float x, float y) => heightMap.GetHeight(x, y);

        public void MakePlayer(Loader loader, string OBJModel, string texName, string texFormat, IPlayerController controller)
        {
            player = new Player(BoundingBox.Default, controller, loader.FromOBJ(OBJModel, texName, texFormat),
                newPosition(0, 0, 0), Vector3.Zero, 1f, 0.5f * new Vector3(1, 2, 1));
        }

        private void CreateTerrain(Loader loader)
        {
            terrain = Generator.HeightMapTerrain(loader, "grass", "png", 1f, GetHeight);
        }

        private void CreateObstacles(Loader loader, params Behavior[] behaviors)
        {
            obstacles = new List<Entity>()
            {
                new Entity(BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "minecraft_stone", "jpg"),
                    newPosition( 3.5f, 0.0f, 0.5f), Vector3.Zero, 1),

                new Entity(BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "minecraft_stone", "jpg"),
                    newPosition( 3.5f, 4.5f, 0.5f), Vector3.Zero, 1),

                new Entity(BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "minecraft_stone", "jpg"),
                    newPosition( 9.5f, 0.0f,-6.0f), Vector3.Zero, 2),

                new Entity(BoundingBox.Default, Behavior.DoNothing, loader.FromOBJ("cube", "minecraft_stone", "jpg"),
                    newPosition( 7.5f, 0.0f, 9.5f), Vector3.Zero, 3),

                new Entity(BoundingBox.Default, behaviors[0], loader.FromOBJ("diamond", "Colors/red", "jpg"),
                    newPosition(-3.5f, 0.0f, 6.5f), Vector3.Zero, 1, new Vector3(1,0.5f,1)),

                new Entity(BoundingBox.Default, behaviors[1], loader.FromOBJ("icosahedron", "Colors/yellow", "jpg"),
                    newPosition(-8.5f, 0.0f,-3.5f), Vector3.Zero, 1),

                new Entity(BoundingBox.Default, behaviors[2], loader.FromOBJ("platform", "wood", "jpg"),
                    newPosition(7,8,2), Vector3.Zero, 1, new Vector3(1,0.25f,2)),
        };
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
                new LightEntity(BoundingBox.None, behaviors[0], light3, lightModel, 0.15f),
                new LightEntity(BoundingBox.None, Behavior.DoNothing, light, lightModel, 10f),
                new LightEntity(BoundingBox.None, behaviors[1], light2, lightModel, 0.2f),
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
