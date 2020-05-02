using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Terrain : Entity
    {
        public Terrain(TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : base(BoundingBox.None, Behavior.DoNothing, texturedModel, position, rotation, scale)
        { }

        public Terrain(TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(BoundingBox.None, Behavior.DoNothing, texturedModel, position, rotation, scale, dimensions)
        { }

        public override void Collision(CollisionInfo cInfo)
        { }
    }
}
