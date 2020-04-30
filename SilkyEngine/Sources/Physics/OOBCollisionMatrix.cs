using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics
{
    public class OOBCollisionMatrix
    {
        private bool[,] computed = new bool[3, 3];
        private float[,] data = new float[3, 3];
        private Vector3[] A, B;

        public OOBCollisionMatrix(Vector3[] A, Vector3[] B)
        {
            this.A = A;
            this.B = B;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    computed[i, j] = false;
                }
            }
        }

        public float this[int i, int j]
        {
            get
            {
                if (computed[i, j]) return data[i, j];

                float tmp = Vector3.Dot(A[i], B[j]);
                data[i, j] = tmp;
                computed[i, j] = true;
                return tmp;
            }
        }
    }
}
