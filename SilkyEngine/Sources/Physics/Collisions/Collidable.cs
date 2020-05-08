using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class Collidable
    {
        public Entity Entity { get; }
        public BoundingVolume BoundingVolume { get; }
        public List<EntityCollisionInfo> collisionInfos { get; } = new List<EntityCollisionInfo>();

        public Collidable(Entity entity, BoundingVolume boundingVolume)
        {
            Entity = entity;
            BoundingVolume = boundingVolume;
        }

        public void IsCollidingWith(Collidable collidable)
        {
            if ((Entity is Movable) && (collidable.Entity is Terrain terrain))
            {
                if (terrain.GetHeight(Entity.Position) >= Entity.Position.Y)
                {
                    collisionInfos.Add(new EntityCollisionInfo(terrain, -Vector3.UnitY));
                }

                return;
            }

            if (!BoundingVolume.Overlaps(collidable.BoundingVolume, out Vector3 normal)) return;
            foreach (var cInfo in collisionInfos)
                if (cInfo.Entity == collidable.Entity) return;

            collisionInfos.Add(new EntityCollisionInfo(collidable.Entity, normal));
        }
    }
}
