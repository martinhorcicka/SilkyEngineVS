using MathNet.Numerics.Providers.LinearAlgebra;
using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public class Collidable : IComparable<Collidable>
    {
        public Entity Entity { get; }
        public BoundingVolume BoundingVolume { get; }
        public List<EntityCollisionInfo> CollisionInfos { get; } = new List<EntityCollisionInfo>();

        public Collidable(Entity entity, BoundingVolume boundingVolume)
        {
            Entity = entity;
            BoundingVolume = boundingVolume;
        }

        public static void IsCollidingWith(Collidable c1, Collidable c2)
        {
            foreach (var cInfo in c1.CollisionInfos)
                if (cInfo.Entity == c2.Entity) return;

            foreach (var cInfo in c2.CollisionInfos)
                if (cInfo.Entity == c1.Entity) return;

            Vector3 normal; Movable movable; Terrain terrain;
            if ((c1.Entity is Movable) && (c2.Entity is Terrain))
            {
                movable = (Movable)c1.Entity; terrain = (Terrain)c2.Entity;
                if (!terrain.Contains(movable.Position) || terrain.GetHeight(movable.Position) < movable.Position.Y) return;

                normal = -Vector3.UnitY;
            }
            else if ((c2.Entity is Movable) && (c1.Entity is Terrain))
            {
                movable = (Movable)c2.Entity; terrain = (Terrain)c1.Entity;
                if (!terrain.Contains(movable.Position) || terrain.GetHeight(movable.Position) < movable.Position.Y) return;

                normal = -Vector3.UnitY;
            }
            else
            {
                if (!c1.BoundingVolume.Overlaps(c2.BoundingVolume, out normal)) return;
            }

            var eCI1 = new EntityCollisionInfo(c2.Entity, normal);
            var eCI2 = new EntityCollisionInfo(c1.Entity, -normal);

            c1.CollisionInfos.Add(eCI1);
            c2.CollisionInfos.Add(eCI2);
        }

        public int CompareTo(Collidable obj)
        {
            switch (Entity)
            {
                case Terrain t:
                    if (obj.Entity is Terrain) return 0;
                    return 1;
                case Obstacle o:
                    if (obj.Entity is Terrain) return -1;
                    if (obj.Entity is Obstacle) return 0;
                    return 1;
                case Movable m:
                    if (obj.Entity is Movable) return 0;
                    return -1;
                default:
                    return 0;
            }
        }
    }
}
