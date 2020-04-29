using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilkyEngine.Sources.Physics
{
    public static class CollisionDetection
    {
        public static event Action<CollisionEventArgs> Collision;

        private static List<Tuple<Entity, BoundingBox>> boxes = new List<Tuple<Entity, BoundingBox>>();
        public static void Add(Entity entity, BoundingBox box) => boxes.Add(Tuple.Create(entity, box));

        public static void CheckForCollisions(double deltaTime)
        {
            foreach (var b1 in boxes)
            {
                foreach (var b2 in boxes)
                {
                    if (b1.Item1 == b2.Item1) continue;

                    if (BoundingBox.CenterDistance(b1.Item2, b2.Item2) < 1.3f)
                        Collision?.Invoke(new CollisionEventArgs(b1, b2, deltaTime));
                }
            }
        }
    }
}
