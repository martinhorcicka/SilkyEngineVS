using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Tools;
using System.Collections.Generic;
using System.Numerics;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class BoundingBox : BoundingVolume
    {
        public static BoundingBox Default { get; } = new BoundingBox(Vector3.Zero, Vector3.Zero, 0);
        public static BoundingBox None { get; } = new BoundingBox(Vector3.Zero, Vector3.Zero, 0);

        private static Vector3[] DefaultVertices = MakeDefaultVertices();

        private static Vector3[] MakeDefaultVertices()
        {
            List<Vector3> vertices = new List<Vector3>(27);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        vertices.Add(new Vector3(i - 1, j - 1, k - 1));
                    }
                }
            }
            vertices.Remove(Vector3.Zero);

            return vertices.ToArray();
        }

        private BoundingBox(Vector3 center, Vector3 rotation, float sideLength) : this(center, rotation, sideLength * Vector3.One)
        { }

        private BoundingBox(Vector3 center, Vector3 rotation, Vector3 dimensions)
            : base(center, rotation, dimensions) { }

        public Vector3[] GetVertices()
        {
            Vector3[] vertices = new Vector3[DefaultVertices.Length];
            Matrix4x4 scaleMat = Matrix4x4.CreateScale(dimensions);
            Matrix4x4 transformMat = scaleMat * MakeRotationMatrix();
            for (int i = 0; i < DefaultVertices.Length; i++)
                vertices[i] = Computation.MatMul(transformMat, DefaultVertices[i]) + center;

            return vertices;
        }

        public override BoundingVolume FromEntity(Entity entity)
        {
            var box = new BoundingBox(entity.Center, entity.Rotation, entity.Dimensions);
            CollisionDetection.Add(entity, box);
            return box;
        }

        protected override Vector3 Support(Vector3 axis)
        {
            float highest = float.MinValue;
            Vector3 support = Vector3.Zero;
            var vertices = GetVertices();

            foreach (var v in vertices)
            {
                float dot = Vector3.Dot(v, axis);
                if (dot > highest)
                {
                    highest = dot;
                    support = v;
                }
            }

            return support;
        }

        private Matrix4x4 MakeRotationMatrix()
        {
            Matrix4x4 retMat = Matrix4x4.Identity;
            retMat *= Matrix4x4.CreateRotationX(rotation.X);
            retMat *= Matrix4x4.CreateRotationY(rotation.Y);
            retMat *= Matrix4x4.CreateRotationZ(rotation.Z);
            return retMat;
        }
    }
}
