using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class CollisionInfo
    {
        public CollisionInfo(Entity target, Vector3 normal, double deltaTime)
        {
            Target = target;
            Normal = normal;
            DeltaTime = deltaTime;
        }

        public Entity Target { get; }
        public double DeltaTime { get; }
        public Vector3 Normal { get; }

    }
}
