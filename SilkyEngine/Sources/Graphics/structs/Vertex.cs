using System.Numerics;
using System.Runtime.InteropServices;

namespace SilkyEngine.Sources.Graphics.Structs
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        private Vector3 position;
        private Vector2 texCoords;
        private Vector3 normal;

        public Vertex(Vector3 position, Vector2 texCoords, Vector3 normal)
        {
            this.position = position;
            this.texCoords = texCoords;
            this.normal = normal;
        }
    }
}