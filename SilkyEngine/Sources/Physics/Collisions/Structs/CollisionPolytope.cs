using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions.Structs
{
    internal struct CollisionPolytope
    {
        private List<Vector3> vertices;
        private List<CollisionTriangle> triangles;


        public CollisionPolytope(CollisionSimplex simplex)
        {
            vertices = simplex.Vertices;
            if (vertices.Count == 3) vertices.Add(Vector3.One);
            triangles = new List<CollisionTriangle>()
            {
                new CollisionTriangle(0, 1, 3),
                new CollisionTriangle(0, 2, 1),
                new CollisionTriangle(2, 0, 3),
                new CollisionTriangle(1, 2, 3),
            };
        }

        public void Insert(Vector3 v)
        {
            List<CollisionEdge> edges = new List<CollisionEdge>();
            void addEdge(int i, int j)
            {
                foreach (var e in edges)
                    if (e.IsOpposite(i, j))
                    {
                        edges.Remove(e);
                        return;
                    }

                edges.Add(new CollisionEdge(i, j));
            };

            int index = vertices.Count;
            vertices.Add(v);

            var trianglesToRemove = new List<CollisionTriangle>();
            foreach (var tri in triangles)
            {
                Vector3 a = vertices[tri.P], b = vertices[tri.Q], c = vertices[tri.R];
                Vector3 norm = ComputeNormal(b - a, c - a, a);
                if (Vector3.Dot(norm, v - a) > 0)
                {
                    addEdge(tri.P, tri.Q);
                    addEdge(tri.Q, tri.R);
                    addEdge(tri.R, tri.P);
                    trianglesToRemove.Add(tri);
                }
            }
            triangles.RemoveAll((t) => trianglesToRemove.Contains(t));
            trianglesToRemove.Clear();

            foreach (var e in edges)
            {
                triangles.Add(new CollisionTriangle(index, e.A, e.B));
            }

            edges.Clear();
        }

        public CollisionPlane FindClosestPlane()
        {
            var closestPlane = new CollisionPlane
            {
                Distance = float.MaxValue
            };
            foreach (var tri in triangles)
            {
                Vector3 a = vertices[tri.P], b = vertices[tri.Q], c = vertices[tri.R];
                Vector3 norm = ComputeNormal(b - a, c - a, a);
                float dist = Vector3.Dot(norm, a);
                if (dist < closestPlane.Distance)
                {
                    closestPlane.Distance = dist;
                    closestPlane.Normal = norm;
                }
            }

            return closestPlane;
        }

        private Vector3 ComputeNormal(Vector3 edge1, Vector3 edge2, Vector3 dir)
        {
            Vector3 norm = Vector3.Cross(edge1, edge2);
            return Vector3.Normalize(norm * MathF.Sign(Vector3.Dot(dir, norm)));
        }
    }
}
