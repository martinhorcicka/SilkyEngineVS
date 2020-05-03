using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    class Plane
    {
        public float Distance { get; set; }
        public Vector3 Normal { get; set; }
        public Triangle Triangle { get; set; }
    }
}
