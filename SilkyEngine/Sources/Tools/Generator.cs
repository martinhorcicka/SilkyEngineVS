using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Graphics.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace SilkyEngine.Sources.Tools
{
    public static class Generator
    {
        public static Entity HeightMapTerrainEntity(int num, Loader loader, string texName, string format, Rectangle area, float density, Func<float, float, float> HeightMap)
        {
            string name = HeightMap.ToString() + "Terrain" + num.ToString();
            TexturedModel model = new TexturedModel(loader.LoadRawModel(name, () => HeightMapTerrainVertices(area, density, HeightMap)), loader.LoadTexture(format, texName));
            return new Entity(Behavior.DoNothing, model, new Vector3(area.Location.X, 0, area.Location.Y), Vector3.Zero, 1f);
        }

        public static List<Entity> HeightMapTerrain(Loader loader, string texName, string format, float density, Func<float, float, float> HeightMap)
        {
            Func<Point, Point> inX = p => { ++p.X; return p; }, inY = p => { ++p.Y; return p; };
            int N = 4;
            int stepSize = 20;
            Point p = new Point(-N * stepSize / 2, -N * stepSize / 2);
            Size s = new Size(stepSize, stepSize);

            var terrainPlanes = new List<Entity>(N * N);
            Point varPoint = p;
            for (int i = 0; i < N; i++)
            {
                varPoint.Y = p.Y;
                for (int j = 0; j < N; j++)
                {

                    terrainPlanes.Add(
                        HeightMapTerrainEntity(i * N + j, loader, texName, format, new Rectangle(varPoint, s), density, HeightMap)
                    );
                    varPoint.Y += stepSize;
                }
                varPoint.X += stepSize;
            }

            return terrainPlanes;
        }
        private static Vertex[] HeightMapTerrainVertices(Rectangle area, float density, Func<float, float, float> HeightMap)
        {
            int numX = (int)(density * area.Width);
            int numZ = (int)(density * area.Height);

            List<Vector3> positions = new List<Vector3>((numX + 1) * (numZ + 1));
            List<Vector2> texCoords = new List<Vector2>((numX + 1) * (numZ + 1));
            List<Vector3> normals = new List<Vector3>((numX + 1) * (numZ + 1));

            for (int j = 0; j <= numZ; j++)
            {
                for (int i = 0; i <= numX; i++)
                {
                    float spacing = 1 / density;
                    Vector3 pos = new Vector3(i, 0, j) * spacing;
                    float x = spacing * i + area.Left;
                    float z = spacing * j + area.Top;
                    pos.Y = HeightMap(x, z);
                    Vector2 uv = new Vector2((float)i / numX, (float)j / numZ);
                    Vector3 normal = ComputeNormal(x, z, spacing, HeightMap);
                    positions.Add(pos);
                    texCoords.Add(uv);
                    normals.Add(normal);
                }
            }

            List<OBJTriangle> triangles = new List<OBJTriangle>(2 * numX * numZ);
            Func<int, int, int> listIndex = (i, j) => j * (numX + 1) + i;
            for (int j = 0; j < numZ; j++)
            {
                for (int i = 0; i < numX; i++)
                {
                    int[] v00 = { listIndex(i, j), listIndex(i, j), listIndex(i, j) };
                    int[] v10 = { listIndex(i + 1, j), listIndex(i + 1, j), listIndex(i + 1, j) };
                    int[] v01 = { listIndex(i, j + 1), listIndex(i, j + 1), listIndex(i, j + 1) };
                    int[] v11 = { listIndex(i + 1, j + 1), listIndex(i + 1, j + 1), listIndex(i + 1, j + 1) };

                    triangles.Add(new OBJTriangle(v00, v11, v10));
                    triangles.Add(new OBJTriangle(v00, v01, v11));
                }
            }

            return Loader.DataToVertices(positions, texCoords, normals, triangles);
        }

        private static Vector3 ComputeNormal(float x, float y, float spacing, Func<float, float, float> HeightMap)
        {
            float heightR = HeightMap(x + spacing, y);
            float heightL = HeightMap(x - spacing, y);
            float heightD = HeightMap(x, y + spacing);
            float heightU = HeightMap(x, y - spacing);
            Vector3 normal = new Vector3(heightL - heightR, 2, heightD - heightU);
            return Vector3.Normalize(normal);

        }
    }
}