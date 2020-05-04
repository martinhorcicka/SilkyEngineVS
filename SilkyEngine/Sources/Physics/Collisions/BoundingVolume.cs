using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Tools;
using System;
using System.Collections.Generic;
using SilkyEngine.Sources.Physics.Collisions.Structs;
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

        private bool DebugOn = false;
        private static int GJK_MAX_IT = 100;
        public bool GJKOverlap(BoundingVolume volume, out Vector3 normal)
        {
            normal = Vector3.UnitY;
            Vector3 support(Vector3 d) => Support(d) - volume.Support(-d);
            CollisionSimplex simplex = new CollisionSimplex();
            Vector3 d = Vector3.One;
            var A = support(d);
            simplex.Add(A);
            d = -A;

            int i = 0;
            while (true)
            {
                i++;
                simplex.Add(support(d));
                if (simplex.DotWithLast(d) <= 0) return false;
                if (simplex.ContainsOrigin(ref d))
                {
                    if (DebugOn) Console.WriteLine($"GJK took {i} iteration" + (i == 1 ? "" : "s") + ".");
                    if (!EPA(simplex, support, out normal)) return false;
                    return true;
                }
                if (i == GJK_MAX_IT)
                    break;
            }

            if (DebugOn) Console.WriteLine($"Maximum number ({GJK_MAX_IT}) of GJK iteraion reached!");
            return false;
        }

        private static float GJK_TOLERANCE = 1e-4f;
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
                if (d.LengthSquared() < GJK_TOLERANCE) return false;
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
        private static int EPA_MAX_IT = 100;
        private bool EPA(CollisionSimplex simplex, Func<Vector3, Vector3> supportFunc, out Vector3 normal)
        {
            CollisionPolytope polytope = new CollisionPolytope(simplex);
            int i = 0;
            normal = Vector3.UnitY;
            while (true)
            {
                i++;
                CollisionPlane p = polytope.FindClosestPlane();
                Vector3 point = supportFunc(p.Normal);
                double dist = Vector3.Dot(point, p.Normal);
                if (dist - p.Distance < EPA_TOLERANCE)
                {
                    normal = p.Normal;
                    if (DebugOn) Console.WriteLine($"EPA took {i} iteration" + (i == 1 ? "" : "s") + ".");
                    return true;
                }
                else
                {
                    polytope.Insert(point);
                }

                if (i == EPA_MAX_IT)
                    break;
            }
            if (DebugOn) Console.WriteLine($"Maximum number ({EPA_MAX_IT}) of EPA iteraion reached!");
            return false;
        }
    }
}
