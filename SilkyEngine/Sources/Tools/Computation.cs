using System.Numerics;

namespace SilkyEngine.Sources.Tools
{
    public static class Computation
    {
        public static Vector3 MatMul(Matrix4x4 m, Vector3 v)
        {
            Vector3 retVec;
            retVec.X = m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z;
            retVec.Y = m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z;
            retVec.Z = m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z;

            return retVec;
        }
    }
}