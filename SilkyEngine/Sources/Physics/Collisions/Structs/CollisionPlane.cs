using SilkyEngine.Sources.Physics.Collisions.Structs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions.Structs
{
    internal struct CollisionPlane
    {
        public float Distance { get; set; }
        public Vector3 Normal { get; set; }
    }
}
