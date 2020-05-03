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

    }
}
