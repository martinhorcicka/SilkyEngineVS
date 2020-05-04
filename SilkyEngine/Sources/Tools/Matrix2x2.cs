using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Tools
{
    public struct Matrix2x2
    {
        private float[,] data;

        public Matrix2x2(float[,] data) => this.data = data;
        public static Matrix2x2 FromVectors(Vector3 A, Vector3 B)
        {
            var data = new float[3, 3];
            data[0, 0] = Vector3.Dot(A, A); data[0, 1] = Vector3.Dot(A, B);
            data[1, 0] = Vector3.Dot(B, A); data[1, 1] = Vector3.Dot(B, B);

            return new Matrix2x2(data);
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
            float det = Determinant();
            Vector2 row1 = new Vector2(data[1, 0], -data[1, 1]);
            Vector2 row2 = new Vector2(-data[0, 0], data[0, 1]);
            SetRow(0, row1 / det);
            SetRow(1, row2 / det);
        }

        public Vector2 Solve(Vector2 RHS) => Solve(RHS.X, RHS.Y);

        public Vector2 Solve(float RHSX, float RHSY)
        {
            float det = Determinant();
            Vector2 result;
            result.X = (data[1, 0] * RHSX - data[1, 1] * RHSY) / det;
            result.Y = (-data[0, 0] * RHSX + data[0, 1] * RHSY) / det;

            return result;
        }

        public float Determinant()
        {
            float result = 0;
            result += data[0, 0] * data[1, 1];
            result -= data[1, 0] * data[0, 1];

            return result;
        }

        public Vector2 Column(int i)
        {
            Vector2 column;
            column.X = data[0, i];
            column.Y = data[1, i];
            return column;
        }
        public Vector2 Row(int i)
        {
            Vector2 row;
            row.X = data[i, 0];
            row.Y = data[i, 1];
            return row;
        }

        private void SetRow(int i, Vector2 row)
        {
            data[i, 0] = row.X;
            data[i, 1] = row.Y;
        }

    }
}
