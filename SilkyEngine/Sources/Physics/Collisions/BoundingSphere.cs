using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class BoundingSphere : BoundingVolume
    {
        public static BoundingSphere Default { get; } = new BoundingSphere(Vector3.Zero, Vector3.Zero, 0);
        new public static BoundingSphere None { get; } = new BoundingSphere(Vector3.Zero, Vector3.Zero, 0);

        private BoundingSphere(Vector3 center, Vector3 rotation, float radius)
            : base(center, rotation, radius)
        {
        }

        private BoundingSphere(Vector3 center, Vector3 rotation, Vector3 dimensions)
            : base(center, rotation, dimensions)
        {

        }

        public override BoundingVolume FromEntity(Entity entity)
        {
            var sphere = new BoundingSphere(entity.Center, entity.Rotation, entity.Scale);
            CollisionDetection.Add(entity, sphere);
            return sphere;
        }

        protected override Vector3 Support(Vector3 d)
        {
            return Vector3.Normalize(d) * dimensions + center;
        }
    }
}
