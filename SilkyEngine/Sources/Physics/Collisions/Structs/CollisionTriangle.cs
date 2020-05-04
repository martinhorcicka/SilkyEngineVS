namespace SilkyEngine.Sources.Physics.Collisions.Structs
{
    internal struct CollisionTriangle
    {
        public readonly int P, Q, R;

        public CollisionTriangle(int p, int q, int r)
        {
            P = p;
            Q = q;
            R = r;
        }
    }
}