using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;
using SilkyEngine.Sources.Graphics.Structs;

namespace SilkyEngine.Sources.Graphics
{
    public class Loader : IDisposable
    {
        private Dictionary<string, RawModel> rawModels = new Dictionary<string, RawModel>();
        private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        private List<uint> vbos = new List<uint>();

        private GL gl;

        public Loader(GL gl)
        {
            this.gl = gl;
        }

        public TexturedModel FromOBJ(string objName, string texName, string format)
        {
            return new TexturedModel(LoadRawModel(objName, () => ParseOBJ(objName)), LoadTexture(format, texName));
        }

        public unsafe RawModel LoadRawModel(string rawModelName, Func<Vertex[]> vertexGetter)
        {
            if (rawModels.ContainsKey(rawModelName))
                return rawModels[rawModelName];

            var vertices = vertexGetter();
            var center = Vector3.Zero;
            foreach (var v in vertices)
                center += v.Position;

            center /= vertices.Length;

            uint VAO = gl.GenVertexArray();
            gl.BindVertexArray(VAO);

            uint vbo = gl.GenBuffer();
            vbos.Add(vbo);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (void* data = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(Vertex)), data, BufferUsageARB.StaticDraw);
            }

            gl.EnableVertexAttribArray(0);
            gl.EnableVertexAttribArray(1);
            gl.EnableVertexAttribArray(2);

            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), null);
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)sizeof(Vector3));
            gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(sizeof(Vector2) + sizeof(Vector3)));


            RawModel newRawModel = new RawModel(gl, VAO, vertices.Length, center);
            rawModels.Add(rawModelName, newRawModel);
            return newRawModel;
        }

        public Texture LoadTexture(string format, string name)
        {
            if (textures.ContainsKey(name))
                return textures[name];

            Texture newTexture = new Texture(gl, name + "." + format);
            textures.Add(name, newTexture);
            return newTexture;
        }

        public Shader LoadShader(string name)
        {
            if (shaders.ContainsKey(name))
                return shaders[name];

            Shader shader = new Shader(gl, name);
            shaders.Add(name, shader);
            return shader;
        }

        private static Vertex[] ParseOBJ(string objName)
        {
            List<Vector3> positions = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<OBJTriangle> triangles = new List<OBJTriangle>();

            var fileHandle = new StreamReader("Resources/Models/" + objName + ".obj");
            var ic = CultureInfo.InvariantCulture;

            static bool isPos(string s) => s.StartsWith("v ");
            static bool isTex(string s) => s.StartsWith("vt ");
            static bool isNorm(string s) => s.StartsWith("vn ");
            static bool isTri(string s) => s.StartsWith("f ");
            static bool isValid(string s) => isPos(s) || isTex(s) || isNorm(s) || isTri(s);

            while (!fileHandle.EndOfStream)
            {
                string line = fileHandle.ReadLine();
                if (!isValid(line))
                    continue;

                if (isTri(line))
                {
                    triangles.Add(new OBJTriangle(line));
                    continue;
                }

                string[] words = line.Split(' ');
                if (isPos(line) || isNorm(line))
                {
                    var tmp = new Vector3(float.Parse(words[1], ic), float.Parse(words[2], ic), float.Parse(words[3], ic));
                    if (isPos(line)) positions.Add(tmp);
                    if (isNorm(line)) normals.Add(tmp);
                    continue;
                }
                if (isTex(line))
                {
                    texCoords.Add(new Vector2(float.Parse(words[1], ic), float.Parse(words[2], ic)));
                    continue;
                }
            }

            return DataToVertices(positions, texCoords, normals, triangles);
        }

        public static Vertex[] DataToVertices(List<Vector3> positions, List<Vector2> texCoords, List<Vector3> normals, List<OBJTriangle> triangles)
        {
            List<Vertex> vertices = new List<Vertex>();
            Vertex makeVertex(int[] vert) => new Vertex(positions[vert[0]], texCoords[vert[1]], normals[vert[2]]);
            foreach (var tri in triangles)
            {
                vertices.Add(makeVertex(tri.v1));
                vertices.Add(makeVertex(tri.v2));
                vertices.Add(makeVertex(tri.v3));
            }

            return vertices.ToArray();
        }

        public void Dispose()
        {
            foreach (var vbo in vbos)
            {
                gl.DeleteBuffer(vbo);
            }
            foreach (var rawModel in rawModels)
            {
                rawModel.Value.Dispose();
                rawModels.Remove(rawModel.Key);
            }

            foreach (var shader in shaders)
            {
                shader.Value.Dispose();
                shaders.Remove(shader.Key);
            }
        }
    }
}