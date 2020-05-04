using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Tools
{
    public struct Matrix3x3
    {
        private float[,] data;
        public float this[int i, int j]
        {
            get { return data[i, j]; }
            set { data[i, j] = value; }
        }

        public Matrix3x3(float[,] data) => this.data = data;
        public Matrix3x3(Matrix3x3 matrix)
        {
            data = new float[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this[i, j] = matrix[i, j];
                }
            }
        }

        public static Matrix3x3 FromVectors(Vector3 A, Vector3 B, Vector3 C)
        {
            var data = new float[3, 3];
            data[0, 0] = Vector3.Dot(A, A); data[0, 1] = Vector3.Dot(A, B); data[0, 2] = Vector3.Dot(A, C);
            data[1, 0] = Vector3.Dot(B, A); data[1, 1] = Vector3.Dot(B, B); data[1, 2] = Vector3.Dot(B, C);
            data[2, 0] = Vector3.Dot(C, A); data[2, 1] = Vector3.Dot(C, B); data[2, 2] = Vector3.Dot(C, C);

            return new Matrix3x3(data);
        }

        public float Sum()
        {
            float sum = 0;
            foreach (var item in data)
            {
                sum += item;
            }
            return sum;
        }

        public void Invert()
        {
            Vector3 x0 = Column(0);
            Vector3 x1 = Column(1);
            Vector3 x2 = Column(2);
            Vector3 row0 = Vector3.Cross(x1, x2);
            float det = Vector3.Dot(row0, x0);
            SetRow(0, row0 / det);
            SetRow(1, Vector3.Cross(x2, x0) / det);
            SetRow(2, Vector3.Cross(x0, x1) / det);
        }

        public Matrix3x3 Inverted(Matrix3x3 matrix)
        {
            Matrix3x3 invMat = new Matrix3x3(matrix);
            invMat.Invert();
            return invMat;
        }


        public Vector3 Solve(float RHSX, float RHSY, float RHSZ) => Solve(new Vector3(RHSX,RHSY,RHSZ));
        public Vector3 Solve(Vector3 RHS) 
        {
            return Computation.MatMul(Inverted(this), RHS);
        }

        public float Determinant()
        {
            float result = 0;
            result += data[0, 0] * data[1, 1] * data[2, 2];
            result += data[0, 1] * data[1, 2] * data[2, 0];
            result += data[1, 0] * data[2, 1] * data[0, 2];

            result -= data[2, 0] * data[1, 1] * data[0, 2];
            result -= data[1, 0] * data[0, 1] * data[2, 2];
            result -= data[2, 1] * data[1, 2] * data[0, 0];

            return result;
        }

        public Vector3 Column(int i)
        {
            Vector3 column;
            column.X = data[0, i];
            column.Y = data[1, i];
            column.Z = data[2, i];
            return column;
        }
        public Vector3 Row(int i)
        {
            Vector3 row;
            row.X = data[i, 0];
            row.Y = data[i, 1];
            row.Z = data[i, 2];
            return row;
        }

        private void SetRow(int i, Vector3 row)
        {
            data[i, 0] = row.X;
            data[i, 1] = row.Y;
            data[i, 2] = row.Z;
        }

    }
}
