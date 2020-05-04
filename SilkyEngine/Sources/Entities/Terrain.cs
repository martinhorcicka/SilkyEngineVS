using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Terrain : Entity
    {
        private HeightMap heightMap;

        public Terrain(HeightMap heightMap, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(heightMap, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }

        public Terrain(HeightMap heightMap, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(BoundingBox.None, Behavior.DoNothing, texturedModel, position, rotation, scale, dimensions)
        {
            this.heightMap = heightMap;
        }

        public float GetHeigt(float x, float y) => heightMap.GetHeight(x, y);
        public bool TryGetHeigt(float x, float y, out float height) => heightMap.TryGetHeight(x, y, out height);

        public override void Collision(CollisionInfo cInfo)
        { }
    }
}
