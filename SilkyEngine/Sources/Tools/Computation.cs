using MathNet.Numerics.Integration;
using System;
using System.Numerics;

namespace SilkyEngine.Sources.Tools
{
    public static class Computation
    {
        public static Func<float, float, float> BumpFunction = (x, y) => (x * x + y * y < 1) ? MathF.Exp(-1 / (1 - x * x - y * y)) : 0;
        private static float mollifierNormalizationConstant = Integrate2D(BumpFunction);
        public static Func<float, float, float> Mollifier = (x, y) => BumpFunction(x, y) / mollifierNormalizationConstant;
        public static Func<float, float, float, float> EMollifier = (eps, x, y) => Mollifier(x / eps, y / eps) / (eps * eps);
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
    }
}