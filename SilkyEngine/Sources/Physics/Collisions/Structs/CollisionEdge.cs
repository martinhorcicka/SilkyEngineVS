namespace SilkyEngine.Sources.Physics.Collisions.Structs
{
    internal struct CollisionEdge
    {
        public readonly int A, B;

        public CollisionEdge(int a, int b)
        {
            A = a;
            B = b;
        }

        public bool IsOpposite(int A, int B) => B == this.A && A == this.B;
    }
}
