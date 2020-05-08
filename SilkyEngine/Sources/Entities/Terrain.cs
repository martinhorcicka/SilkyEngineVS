using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Terrain : Entity
    {
        private RectangleF area;
        public Terrain(RectangleF area, World world, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(area, world, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }

        public Terrain(RectangleF area, World world, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(world, BoundingBox.None, Behavior.DoNothing, texturedModel, position, rotation, scale, dimensions)
        {
            this.area = area;
        }

        public bool Contains(Vector3 position) => area.Contains(position.X, position.Z);
        public float GetHeight(Vector3 position) => world.GetHeight(position);
        public float GetHeight(float x, float y) => world.GetHeight(x, y);

        public override void Collision(List<EntityCollisionInfo> collisionInfos)
        { }
    }
}
