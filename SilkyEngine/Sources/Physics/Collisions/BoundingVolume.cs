using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Tools;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public abstract class BoundingVolume
    {
        public virtual bool Overlaps(BoundingVolume volume, out Vector3 normal) => GJKOverlap(volume, out normal);

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

        protected bool GJKOverlap(BoundingVolume volume, out Vector3 normal)
        {
            normal = Vector3.UnitY;
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
                if (simplex.ContainsOrigin(ref d))
                {
                    EPA(simplex, support, out normal);
                    return true;
                }
            }
        }


        private static float EPA_TOLERANCE = 1e-4f;
        private void EPA(CollisionSimplex simplex, Func<Vector3, Vector3> supportFunc, out Vector3 normal)
        {
            while (true)
            {
                Plane p = simplex.FindClosestPlane();
                Vector3 point = supportFunc(p.Normal);
                double dist = Vector3.Dot(point, p.Normal);
                if (dist - p.Distance < EPA_TOLERANCE)
                {
                    normal = p.Normal;
                    break;
                }
                else
                {
                    simplex.Insert(point, p.Triangle);
                }
            }
        }
    }
}
