using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Physics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public static class CollisionDetection
    {
        private static List<Tuple<Entity, BoundingVolume>> collidables = new List<Tuple<Entity, BoundingVolume>>();
        public static void Add(Entity entity, BoundingVolume volume) => collidables.Add(Tuple.Create(entity, volume));

        public static void CheckCollisions() => CheckCollisions(collidables);

        private static void CheckCollisions(List<Tuple<Entity, BoundingVolume>> collidables)
        {
            for (int i = 0; i < collidables.Count; i++)
                for (int j = i + 1; j < collidables.Count; j++)
                {
                    var c1 = collidables[i];
                    var c2 = collidables[j];
                    if (AreColliding(c1.Item2, c2.Item2, out Vector3 normal))
                        DispatchCollision(c1.Item1, c2.Item1, normal);
                }
        }

        private static bool AreColliding(BoundingVolume v1, BoundingVolume v2, out Vector3 normal)
        {
            return v1.Overlaps(v2, out normal);
        }

        private static void DispatchCollision(Entity e1, Entity e2, Vector3 normal)
        {
            e1.Collision(new CollisionInfo(e2, normal));
            e2.Collision(new CollisionInfo(e1, -normal));
        }
    }
}
