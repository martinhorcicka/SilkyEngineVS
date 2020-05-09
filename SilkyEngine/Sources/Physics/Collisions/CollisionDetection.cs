using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Physics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SilkyEngine.Sources.Physics.Collisions
{
    public static class CollisionDetection
    {
        private static List<Collidable> collidables = new List<Collidable>();

        public static void Add(Entity entity, BoundingVolume volume)
        {
            collidables.Add(new Collidable(entity, volume));
            collidables.Sort();
        }
        public static void AddTerrain(List<Terrain> terrain)
        {
            foreach (var t in terrain)
            {
                Add(t, BoundingBox.None);
            }
        }

        public static void CheckCollisions()
        {
            CheckCollisions(collidables);
        }

        private static void CheckCollisions(List<Collidable> collidables)
        {
            for (int i = 0; i < collidables.Count; i++)
                for (int j = i + 1; j < collidables.Count; j++)
                {
                    var c1 = collidables[i];
                    var c2 = collidables[j];
                    Collidable.IsCollidingWith(c1, c2);
                }

            DispatchCollision(collidables);
        }

        private static void DispatchCollision(List<Collidable> collidables)
        {
            foreach (var collidable in collidables)
            {
                collidable.Entity.Collision(collidable.CollisionInfos);
                collidable.CollisionInfos.Clear();
            }
        }
    }
}
