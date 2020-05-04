using SilkyEngine.Sources.Tools;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    internal class CollisionSimplex
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<Triangle> triangles;

        public void Add(Vector3 v) => vertices.Add(v);
        public void Insert(Vector3 v, Triangle triangle)
        {
            int index = vertices.Count;
            vertices.Add(v);
            triangles.Remove(triangle);
            triangles.Add(new Triangle(index, triangle.P, triangle.Q));
            triangles.Add(new Triangle(index, triangle.Q, triangle.R));
            triangles.Add(new Triangle(index, triangle.R, triangle.P));
        }
        public float DotWithLast(Vector3 d) => Dot(vertices[^1], d);

        public Vector3 ClosestPointToOrigin(Vector3 newPoint)
        {
            if (vertices.Count != 3)
                return Vector3.Zero;
            Vector3 a = vertices[0], b = vertices[1], c = vertices[2];

            if (newPoint == Vector3.Zero)
                return ClosestPointToOrigin(a, b, c);


            vertices.Add(newPoint);
            Vector3 pc = ClosestPointToOrigin(a, b, newPoint);
            Vector3 pb = ClosestPointToOrigin(a, c, newPoint);
            Vector3 pa = ClosestPointToOrigin(c, b, newPoint);

            if (pa.LengthSquared() < pb.LengthSquared())
            {
                if (pa.LengthSquared() < pc.LengthSquared())
                {
                    vertices.Remove(a);
                    return pa;
                }
            }
            else
            {
                if (pb.LengthSquared() < pc.LengthSquared())
                {
                    vertices.Remove(b);
                    return pb;
                }
            }

            vertices.Remove(c);
            return pc;
        }

        private Vector3 ClosestPointToOrigin(Vector3 a, Vector3 b, Vector3 c)
        {
            Func<Tuple<float, float>, Vector3> F = (p) => a + p.Item1 * (b - a) + p.Item2 * (c - a);
            Func<Tuple<float, float>, float> f = (p) => F(p).LengthSquared();
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
            if (tPlusSIsOne > 0 && tPlusSIsOne < 1) criticalPoints.Add(Tuple.Create(tPlusSIsOne, 1-tPlusSIsOne));
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

        //private Vector3 ClosestPointToOrigin(Vector3 a, Vector3 b, Vector3 c)
        //{
        //    Vector3 MakeX(Vector3 bary) => bary.X * a + bary.Y * b + bary.Z * c;
        //    Matrix3x3 mat = Matrix3x3.FromVectors(a, b, c);
        //    mat.Invert();
        //    float nu = 1 / mat.Sum();
        //    Vector3 p;
        //    Vector3 column0 = mat.Column(0), column1 = mat.Column(1), column2 = mat.Column(2);
        //    p = column0 + column1 + column2;
        //    p *= nu;

        //    float mu1, mu2, mu3;
        //    try
        //    {
        //        mu1 = -p.X / column0.X;
        //    }
        //    catch (DivideByZeroException)
        //    {
        //        mu1 = 0;
        //    }
        //    try
        //    {
        //        mu2 = -p.Y / column1.Y;
        //    }
        //    catch (DivideByZeroException)
        //    {
        //        mu2 = 0;
        //    }
        //    try
        //    {
        //        mu3 = -p.Z / column2.Z;
        //    }
        //    catch (DivideByZeroException)
        //    {
        //        mu3 = 0;
        //    }

        //    List<Vector3> critialPoints = new List<Vector3>()
        //    {
        //        p + mu1 * column0,
        //        p + mu2 * column1,
        //        p + mu3 * column2,
        //        Vector3.UnitX,
        //        Vector3.UnitY,
        //        Vector3.UnitZ,
        //    };
        //    if (Vec3Min(p) >= 0 && Vec3Max(p) <= 1)
        //    {
        //        critialPoints.Add(p);
        //    }

        //    Vector3 X = Vector3.Zero;
        //    float dist = float.MaxValue;
        //    foreach (var cp in critialPoints)
        //    {
        //        Vector3 spaceVec = MakeX(cp);
        //        float len = spaceVec.LengthSquared();
        //        if (len < dist)
        //        {
        //            X = spaceVec;
        //            dist = len;
        //        }
        //    }

        //    return X;
        //}

        public bool ContainsOrigin(ref Vector3 direction)
        {
            var a = vertices[^1];
            var ao = -a;
            if (vertices.Count == 4)
            {
                Vector3 b = vertices[0], c = vertices[1], d = vertices[2];
                Vector3 ab = b - a, ac = c - a, ad = d - a;
                Vector3 abcPerp = -ComputeNormal(ab, ac, ad);
                Vector3 acdPerp = -ComputeNormal(ac, ad, ab);
                Vector3 abdPerp = -ComputeNormal(ab, ad, ac);

                if (Dot(ao, abcPerp) > 0)
                {
                    vertices.Remove(d);
                    direction = abcPerp;
                    return false;
                }

                if (Dot(ao, acdPerp) > 0)
                {
                    vertices.Remove(b);
                    direction = acdPerp;
                    return false;
                }

                if (Dot(ao, abdPerp) > 0)
                {
                    vertices.Remove(c);
                    direction = abdPerp;
                    return false;
                }

                triangles = new List<Triangle>()
                {
                    new Triangle(0, 1, 3),
                    new Triangle(0, 2, 1),
                    new Triangle(2, 0, 3),
                    new Triangle(1, 2, 3),
                };

                return true;
            }

            if (vertices.Count == 3)
            {
                direction = ComputeNormal(vertices[0] - a, vertices[1] - a, ao);
                if (direction.LengthSquared() < 1e-4) return true;
                return false;
            }

            var nab = Vector3.Normalize(vertices[0] - a);
            direction = ao - Dot(nab, ao) * nab;

            return false;
        }

        public Plane FindClosestPlane()
        {
            var closestPlane = new Plane();
            closestPlane.Distance = float.MaxValue;
            foreach (var tri in triangles)
            {
                Vector3 a = vertices[tri.P], b = vertices[tri.Q], c = vertices[tri.R];
                Vector3 e1 = b - a, e2 = c - a;
                Vector3 n = Vector3.Normalize(ComputeNormal(e1, e2, a));
                float d = Dot(a, n);
                if (d < closestPlane.Distance)
                {
                    closestPlane.Distance = d;
                    closestPlane.Normal = n;
                    closestPlane.Triangle = tri;
                }
            }

            return closestPlane;
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
