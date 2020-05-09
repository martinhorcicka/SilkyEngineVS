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
            List<Vector3> vertices = new List<Vector3>(8);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (i == 1 || j == 1 || k == 1) continue;
                        vertices.Add(new Vector3(i - 1, j - 1, k - 1));
                    }
                }
            }

            return vertices.ToArray();
        }

        private Vector3[] Vertices;

        public override void Translate(Vector3 dp)
        {
            base.Translate(dp);
            Vertices = GetVertices();
        }
        public override void RotateX(float angle)
        {
            base.RotateX(angle);
            Vertices = GetVertices();
        }
        public override void RotateY(float angle)
        {
            base.RotateY(angle);
            Vertices = GetVertices();
        }
        public override void RotateZ(float angle)
        {
            base.RotateZ(angle);
            Vertices = GetVertices();
        }

        private BoundingBox(Vector3 center, Vector3 rotation, float sideLength) : this(center, rotation, sideLength * Vector3.One)
        { }

        private BoundingBox(Vector3 center, Vector3 rotation, Vector3 dimensions)
            : base(center, rotation, dimensions)
        {
            DefaultVertices ??= MakeDefaultVertices();
            Vertices = GetVertices();
        }

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

            foreach (var v in Vertices)
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

        public override BoundingVolume MakeCore()
        {
            return new BoundingBox(center, rotation, scaleFactor * dimensions);
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
