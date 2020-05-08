using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class CollisionInfo
    {
        public CollisionInfo(Entity target, Vector3 normal)
        {
            Target = target;
            Normal = normal;
        }

        public Entity Target { get; }
        public Vector3 Normal { get; }

    }
}
