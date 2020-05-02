using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class CollisionInfo
    {
        public CollisionInfo(Entity target, double deltaTime)
        {
            Target = target;
            DeltaTime = deltaTime;
        }

        public Entity Target { get; }
        public double DeltaTime { get; }

    }
}
