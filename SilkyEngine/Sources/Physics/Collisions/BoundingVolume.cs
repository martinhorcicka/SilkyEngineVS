using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public abstract class BoundingVolume
    {
        public virtual bool Overlaps(BoundingVolume volume) => GJKOverlap(volume);

        protected Vector3 center;
        protected Vector3 rotation;
        protected Vector3 dimensions;
        public Vector3 Center => center;
        protected abstract Vector3 Support(Vector3 d);

        protected BoundingVolume(Vector3 center, Vector3 rotation, float sideLength) : this(center, rotation, sideLength * Vector3.One)
        { }

        protected BoundingVolume(Vector3 center, Vector3 rotation, Vector3 dimensions)
        {
            this.center = center;
            this.rotation = rotation;
            this.dimensions = dimensions;
        }

        public void Translate(Vector3 dp) => center += dp;
        public void RotateX(float angle) => rotation.X += angle;
        public void RotateY(float angle) => rotation.Y += angle;
        public void RotateZ(float angle) => rotation.Z += angle;
        public void SetHeight(float newHeight) => center.Y = newHeight;

        public abstract BoundingVolume FromEntity(Entity entity);

        protected bool GJKOverlap(BoundingVolume volume)
        {
            Vector3 support(Vector3 d) => Support(d) - volume.Support(-d);
            CollisionSimplex simplex = new CollisionSimplex();
            Vector3 d = Vector3.One;
            var A = support(d);
            simplex.Add(A);
            d = -A;

            while (true)
            {
                simplex.Add(support(d));
                if (simplex.DotWithLast(d) <= 0) return false;
                if (simplex.ContainsOrigin(ref d)) return true;
            }
        }
    }
}
