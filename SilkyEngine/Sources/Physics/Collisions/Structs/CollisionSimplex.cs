using SilkyEngine.Sources.Physics.Collisions.Structs;
using SilkyEngine.Sources.Tools;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions.Structs
{
    internal struct CollisionSimplex
    {
        public List<Vector3> Vertices { get; private set; }

        public void Add(Vector3 v)
        {
            Vertices ??= new List<Vector3>();
            Vertices.Add(v);
        }

        public float DotWithLast(Vector3 d) => Dot(Vertices[^1], d);

        public Vector3 ClosestPointToOrigin(Vector3 newPoint)
        {
            if (Vertices.Count != 3)
                return Vector3.Zero;
            Vector3 a = Vertices[0], b = Vertices[1], c = Vertices[2];

            if (newPoint == Vector3.Zero)
                return ClosestPointToOrigin(a, b, c);


            Vertices.Add(newPoint);
            Vector3 pc = ClosestPointToOrigin(a, b, newPoint);
            Vector3 pb = ClosestPointToOrigin(a, c, newPoint);
            Vector3 pa = ClosestPointToOrigin(c, b, newPoint);

            if (pa.LengthSquared() < pb.LengthSquared())
            {
                if (pa.LengthSquared() < pc.LengthSquared())
                {
                    Vertices.Remove(a);
                    return pa;
                }
            }
            else
            {
                if (pb.LengthSquared() < pc.LengthSquared())
                {
                    Vertices.Remove(b);
                    return pb;
                }
            }

            Vertices.Remove(c);
            return pc;
        }

        private Vector3 ClosestPointToOrigin(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 F(Tuple<float, float> p) => a + p.Item1 * (b - a) + p.Item2 * (c - a);
            List<Tuple<float, float>> criticalPoints = new List<Tuple<float, float>>()
            {
                Tuple.Create(0f,0f),
                Tuple.Create(0f,1f),
                Tuple.Create(1f,0f),
            };
            float sIsZero = -Dot(a, c - a) / (c - a).LengthSquared();
            float tIsZero = -Dot(a, b - a) / (b - a).LengthSquared();
            float tPlusSIsOne = -(Dot(b, c) + Dot(c, c)) / (Dot(b, b) - Dot(c, c) - 2 * Dot(b, c));
            if (sIsZero > 0 && sIsZero < 1) criticalPoints.Add(Tuple.Create(0f, sIsZero));
            if (tIsZero > 0 && tIsZero < 1) criticalPoints.Add(Tuple.Create(tIsZero, 0f));
            if (tPlusSIsOne > 0 && tPlusSIsOne < 1) criticalPoints.Add(Tuple.Create(tPlusSIsOne, 1 - tPlusSIsOne));
            Vector2 st = Matrix2x2.FromVectors(b - a, c - a).Solve(Dot(b - a, a), Dot(c - a, a));
            float s = st.X, t = st.Y;
            if (s > 0 && t > 0 && s + t < 1) criticalPoints.Add(Tuple.Create(s, t));

            Vector3 minimum = Vector3.Zero;
            float dist = float.MaxValue;
            foreach (var cp in criticalPoints)
            {
                Vector3 Point = F(cp);
                float len = Point.LengthSquared();
                if (len < dist)
                {
                    minimum = Point;
                    len = dist;
                }
            }
            return minimum;
        }
        public bool ContainsOrigin(ref Vector3 direction)
        {
            var a = Vertices[^1];
            var ao = -a;
            if (Vertices.Count == 4)
            {
                Vector3 b = Vertices[0], c = Vertices[1], d = Vertices[2];
                Vector3 ab = b - a, ac = c - a, ad = d - a;
                Vector3 abcPerp = -ComputeNormal(ab, ac, ad);
                Vector3 acdPerp = -ComputeNormal(ac, ad, ab);
                Vector3 abdPerp = -ComputeNormal(ab, ad, ac);

                if (Dot(ao, abcPerp) > 0)
                {
                    Vertices.Remove(d);
                    direction = abcPerp;
                    return false;
                }

                if (Dot(ao, acdPerp) > 0)
                {
                    Vertices.Remove(b);
                    direction = acdPerp;
                    return false;
                }

                if (Dot(ao, abdPerp) > 0)
                {
                    Vertices.Remove(c);
                    direction = abdPerp;
                    return false;
                }


                return true;
            }

            if (Vertices.Count == 3)
            {
                direction = ComputeNormal(Vertices[0] - a, Vertices[1] - a, ao);
                if (direction.LengthSquared() < 1e-4) return true;
                return false;
            }

            var nab = Vector3.Normalize(Vertices[0] - a);
            direction = ao - Dot(nab, ao) * nab;

            return false;
        }


        private Vector3 ComputeNormal(Vector3 edge1, Vector3 edge2, Vector3 dir)
        {
            Vector3 norm = Vector3.Cross(edge1, edge2);
            return norm * MathF.Sign(Dot(dir, norm));
        }

        private float Dot(Vector3 a, Vector3 b) => Vector3.Dot(a, b);

        private float Vec3Max(Vector3 a) => -Vec3Min(-a);
        private float Vec3Min(Vector3 a)
        {
            if (a.X < a.Y)
            {
                if (a.X < a.Z)
                    return a.X;
            }
            else
            {
                if (a.Y < a.Z)
                    return a.Y;
            }

            return a.Z;
        }
    }
}
