using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class EntityCollisionInfo
    {
        public Entity Entity { get; }
        public Vector3 Normal { get; }

        public EntityCollisionInfo(Entity entity, Vector3 normal)
        {
            Entity = entity;
            Normal = normal;
        }
    }
}
