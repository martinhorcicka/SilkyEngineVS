using MathNet.Numerics.Integration;
using System;
using System.Linq;
using System.Numerics;

namespace SilkyEngine.Sources.Tools
{
    public static class Computation
    {
        public static Vector2 RandomVector2(Random generator) => Vector2.Normalize(new Vector2((float)generator.NextDouble() - 0.5f, (float)generator.NextDouble() - 0.5f));
        public static Func<float, float, float> BumpFunction = (x, y) => (x * x + y * y < 1) ? MathF.Exp(-1 / (1 - x * x - y * y)) : 0;
        private static float mollifierNormalizationConstant = Integrate2D(BumpFunction);
        public static Func<float, float, float> Mollifier = (x, y) => BumpFunction(x, y) / mollifierNormalizationConstant;
        public static Func<float, float, float, float> EMollifier = (eps, x, y) => Mollifier(x / eps, y / eps) / (eps * eps);
        public static float Average(params float[] nums) => nums.Average();
        public static float RadToDeg(float rad) => rad / MathF.PI * 180f;
        public static float DegToRad(float deg) => deg * MathF.PI / 180f;
        public static float AngleBetweenVectors(Vector3 v, Vector3 w) => MathF.Acos(Vector3.Dot(v, w) / (v.Length() * w.Length()));

        public static Vector3 MatMul(Matrix4x4 m, Vector3 v)
        {
            Vector3 retVec;
            retVec.X = m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z;
            retVec.Y = m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z;
            retVec.Z = m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z;

            return retVec;
        }
        public static Vector3 MatMul(Matrix3x3 m, Vector3 v)
        {
            Vector3 retVec;
            retVec.X = m[0, 0] * v.X + m[0, 1] * v.Y + m[0, 2] * v.Z;
            retVec.Y = m[1, 0] * v.X + m[1, 1] * v.Y + m[1, 2] * v.Z;
            retVec.Z = m[2, 0] * v.X + m[2, 1] * v.Y + m[2, 2] * v.Z;

            return retVec;
        }

        private static float Integrate2D(Func<float, float, float> func)
        {
            Func<double, double, double> fnd = (x, y) => func((float)x, (float)y);
            double result = GaussLegendreRule.Integrate(fnd, -1, 1, -1, 1, 8);

            return (float)result;
        }

        public static float Mollification(Func<float, float, float> func, float t, float s, float eps)
        {
            Func<float, float, float> tmpFunc = (x, y) => EMollifier(eps, x, y) * func(x - t, y - s);
            return Integrate2D(tmpFunc);
        }

        public static Vector3[] GenerateUniformUnitVectors(float angleStep = MathF.PI / 36f)
        {
            int numAroundY = (int)(2 * MathF.PI / angleStep);
            int numBottomToTop = numAroundY / 2 - 1;
            Vector3[] unitVectors = new Vector3[numAroundY * numBottomToTop + 2];
            for (int i = 0; i < numAroundY; i++)
            {
                for (int j = 1; j < numBottomToTop + 1; j++)
                {
                    Vector3 unit;
                    float theta = i * angleStep, phi = j * angleStep;
                    unit.X = MathF.Cos(theta) * MathF.Sin(phi);
                    unit.Y = MathF.Cos(phi);
                    unit.Z = MathF.Sin(theta) * MathF.Sin(phi);
                    unitVectors[i * numBottomToTop + j - 1] = unit;
                }
            }
            unitVectors[^2] = -Vector3.UnitY;
            unitVectors[^1] = Vector3.UnitY;

            return unitVectors;
        }
    }
}