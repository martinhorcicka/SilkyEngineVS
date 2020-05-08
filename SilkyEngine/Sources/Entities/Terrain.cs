using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Terrain : Entity
    {
        public Terrain(World world, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(world, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }

        public Terrain(World world, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(world, BoundingBox.None, Behavior.DoNothing, texturedModel, position, rotation, scale, dimensions)
        {
        }

        public override void Collision(CollisionInfo cInfo)
        { }
    }
}
