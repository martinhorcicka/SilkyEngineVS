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

        protected static float scaleFactor = 0.9f;
        protected Vector3 center;
        protected Vector3 rotation;
        protected Vector3 dimensions;

        public Vector3 Center => center;
        public BoundingVolume Core => MakeCore();

        protected abstract Vector3 Support(Vector3 d);
        public abstract BoundingVolume MakeCore();

        protected BoundingVolume(Vector3 center, Vector3 rotation, float sideLength) : this(center, rotation, sideLength * Vector3.One)
        { }

        protected BoundingVolume(Vector3 center, Vector3 rotation, Vector3 dimensions)
        {
            this.center = center;
            this.rotation = rotation;
            this.dimensions = dimensions;
        }

        public virtual void Translate(Vector3 dp) => center += dp;
        public virtual void RotateX(float angle) => rotation.X += angle;
        public virtual void RotateY(float angle) => rotation.Y += angle;
        public virtual void RotateZ(float angle) => rotation.Z += angle;
        public virtual void SetHeight(float newHeight) => center.Y = newHeight;

        public abstract BoundingVolume FromEntity(Entity entity);

        public bool GJKOverlap(BoundingVolume volume, out Vector3 normal)
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
                    if(!EPA(simplex, support, out normal)) return false;
                    return true;
                }
            }
        }

        public bool GJKDistance(BoundingVolume volume, out Vector3 normal)
        {
            normal = Vector3.UnitY;
            Vector3 support(Vector3 d) => Support(d) - volume.Support(-d);
            CollisionSimplex simplex = new CollisionSimplex();
            Vector3 d = Vector3.One;
            simplex.Add(support(d));
            simplex.Add(support(-d));
            simplex.ContainsOrigin(ref d);
            simplex.Add(support(d));
            d = simplex.ClosestPointToOrigin(Vector3.Zero);
            while (true)
            {
                d = -d;
                if (d.LengthSquared() < 1e-1f) return false;
                var c = support(d);
                float dc = Vector3.Dot(c, d);
                float da = simplex.DotWithLast(d);
                if (dc - da < 1e-1)
                {
                    normal = Vector3.Normalize(d);
                    return true;
                }

                d = simplex.ClosestPointToOrigin(c);
            }
        }


        private static float EPA_TOLERANCE = 1e-4f;
        private static int MAX_EPA_IT = 10;
        private bool EPA(CollisionSimplex simplex, Func<Vector3, Vector3> supportFunc, out Vector3 normal)
        {
            int i = 0;
            normal = Vector3.UnitY;
            while (true)
            {
                Plane p = simplex.FindClosestPlane();
                Vector3 point = supportFunc(p.Normal);
                double dist = Vector3.Dot(point, p.Normal);
                if (dist - p.Distance < EPA_TOLERANCE)
                {
                    normal = p.Normal;
                    Console.WriteLine($"EPA took {i} iterations.");
                    return true;
                }
                else
                {
                    simplex.Insert(point, p.Triangle);
                }

                if (i==MAX_EPA_IT) 
                    break;
                i++;
            }
            Console.WriteLine("Maximum number of EPA iteraion reached!");
            return false;
        }
    }
}
