using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Tools;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics
{
    public class BoundingBox : IBoundingVolume
    {
        public static BoundingBox Default { get; } = null;
        public static BoundingBox None { get; } = new BoundingBox(Vector3.Zero, Vector3.Zero, 0);

        private Vector3 center;
        private Vector3 rotation;
        private Vector3 dimensions;

        private BoundingBox(Vector3 center, Vector3 rotation, float sideLength) : this(center, rotation, sideLength * Vector3.One)
        { }
        private BoundingBox(Vector3 center, Vector3 rotation, Vector3 dimensions)
        {
            this.center = center;
            this.rotation = rotation;
            this.dimensions = dimensions;
        }

        public Vector3 Center
        {
            get { return center; }
            set { center = value; }
        }
        public float[] GetDimensions { get { return new float[] { dimensions.X, dimensions.Y, dimensions.Z }; } }

        public void Translate(Vector3 dp) => center += dp;
        public void RotateX(float angle) => rotation.X += angle;
        public void RotateY(float angle) => rotation.Y += angle;
        public void RotateZ(float angle) => rotation.Z += angle;
        public void SetHeight(float newHeight) => center.Y = newHeight;


        public static BoundingBox FromEntity(Entity entity)
        {
            var box = new BoundingBox(entity.Center, entity.Rotation, entity.Dimensions);
            CollisionDetection.Add(entity, box);
            return box;
        }

        public Vector3[] MakeAxes()
        {
            var rotMat = MakeRotationMatrix();
            Vector3 vec1 = Computation.MatMul(rotMat, Vector3.UnitX);
            Vector3 vec2 = Computation.MatMul(rotMat, Vector3.UnitY);
            Vector3 vec3 = Computation.MatMul(rotMat, Vector3.UnitZ);
            return new Vector3[] { vec1, vec2, vec3 };
        }

        public bool Contains(IBoundingVolume volume) => Contains((BoundingBox)volume);

        private bool Contains(BoundingBox box)
        {
            var A = MakeAxes();
            var a = GetDimensions;
            var B = box.MakeAxes();
            var b = box.GetDimensions;
            Vector3 D = box.Center - center;
            OOBCollisionMatrix C = new OOBCollisionMatrix(A, B);
            for (int i = 0; i < 15; i++)
            {
                if (ComputeRs(i, A, a, B, b, C, D)) return false;
            }
            return true;
        }

        private bool ComputeRs(int i, Vector3[] A, float[] a, Vector3[] B, float[] b, OOBCollisionMatrix C, Vector3 D)
        {
            Func<float, float> abs = MathF.Abs;
            Func<Vector3, Vector3, float> dot = (vec1, vec2) => Vector3.Dot(vec1, vec2);
            float R0, R1, R;
            switch (i)
            {
                case 0:
                    R0 = a[0]; R1 = b[0] * abs(C[0, 0]) + b[1] * abs(C[0, 1]) + b[2] * abs(C[0, 2]); R = abs(dot(A[0], D));
                    break;
                case 1:
                    R0 = a[1]; R1 = b[0] * abs(C[1, 0]) + b[1] * abs(C[1, 1]) + b[2] * abs(C[1, 2]); R = abs(dot(A[1], D));
                    break;
                case 2:
                    R0 = a[2]; R1 = b[0] * abs(C[2, 0]) + b[1] * abs(C[2, 1]) + b[2] * abs(C[2, 2]); R = abs(dot(A[2], D));
                    break;
                case 3:
                    R0 = a[0] * abs(C[0, 0]) + a[1] * abs(C[1, 0]) + a[2] * abs(C[2, 0]); R1 = b[0]; R = abs(dot(B[0], D));
                    break;
                case 4:
                    R0 = a[0] * abs(C[0, 1]) + a[1] * abs(C[1, 1]) + a[2] * abs(C[2, 1]); R1 = b[1]; R = abs(dot(B[1], D));
                    break;
                case 5:
                    R0 = a[0] * abs(C[0, 2]) + a[1] * abs(C[1, 2]) + a[2] * abs(C[2, 2]); R1 = b[2]; R = abs(dot(B[2], D));
                    break;
                case 6:
                    R0 = a[1] * abs(C[2, 0]) + a[2] * abs(C[1, 0]); R1 = b[1] * abs(C[0, 2]) + b[2] * abs(C[0, 1]); R = abs(C[1, 0] * dot(A[2], D) - C[2, 0] * dot(A[1], D));
                    break;
                case 7:
                    R0 = a[1] * abs(C[2, 1]) + a[2] * abs(C[1, 1]); R1 = b[0] * abs(C[0, 2]) + b[2] * abs(C[0, 0]); R = abs(C[1, 1] * dot(A[2], D) - C[2, 1] * dot(A[1], D));
                    break;
                case 8:
                    R0 = a[1] * abs(C[2, 2]) + a[2] * abs(C[1, 2]); R1 = b[0] * abs(C[0, 1]) + b[1] * abs(C[0, 0]); R = abs(C[1, 2] * dot(A[2], D) - C[2, 2] * dot(A[1], D));
                    break;
                case 9:
                    R0 = a[0] * abs(C[2, 0]) + a[2] * abs(C[0, 0]); R1 = b[1] * abs(C[1, 2]) + b[2] * abs(C[1, 1]); R = abs(C[2, 0] * dot(A[0], D) - C[0, 0] * dot(A[2], D));
                    break;
                case 10:
                    R0 = a[0] * abs(C[2, 1]) + a[2] * abs(C[0, 1]); R1 = b[0] * abs(C[1, 2]) + b[2] * abs(C[1, 0]); R = abs(C[2, 1] * dot(A[0], D) - C[0, 1] * dot(A[2], D));
                    break;
                case 11:
                    R0 = a[0] * abs(C[2, 2]) + a[2] * abs(C[0, 2]); R1 = b[0] * abs(C[1, 1]) + b[1] * abs(C[1, 0]); R = abs(C[2, 2] * dot(A[0], D) - C[0, 2] * dot(A[2], D));
                    break;
                case 12:
                    R0 = a[0] * abs(C[1, 0]) + a[1] * abs(C[0, 0]); R1 = b[1] * abs(C[2, 2]) + b[2] * abs(C[2, 1]); R = abs(C[0, 0] * dot(A[1], D) - C[1, 0] * dot(A[0], D));
                    break; // a0 | c10 | +a1 | c00 | b1 | c22 | +b2 | c21 || c00A1·D−c10A0·D
                case 13:
                    R0 = a[0] * abs(C[1, 1]) + a[1] * abs(C[0, 1]); R1 = b[0] * abs(C[2, 2]) + b[2] * abs(C[2, 0]); R = abs(C[0, 1] * dot(A[1], D) - C[1, 1] * dot(A[0], D));
                    break; // a0 | c11 | +a1 | c01 | b0 | c22 | +b2 | c20 || c01A1·D−c11A0·D |
                case 14:
                    R0 = a[0] * abs(C[1, 2]) + a[1] * abs(C[0, 2]); R1 = b[0] * abs(C[2, 1]) + b[1] * abs(C[2, 0]); R = abs(C[0, 2] * dot(A[1], D) - C[1, 2] * dot(A[0], D));
                    break; // a0 | c12 | +a1 | c02 | b0 | c21 | +b1 | c20 || c02A1·D−c12A0·D |
                default:
                    R0 = 0; R1 = 0; R = 1;
                    break;
            }
            return (R0 + R1 < R);
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
