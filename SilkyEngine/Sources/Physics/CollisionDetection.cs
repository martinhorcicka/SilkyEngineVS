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
            for (int i = 0; i < boxes.Count; i++)
            {
                for (int j = i + 1; j < boxes.Count; j++)
                {
                    var b1 = boxes[i];
                    var b2 = boxes[j];
                    if (IsColliding(b1.Item2, b2.Item2))
                        Collision?.Invoke(new CollisionEventArgs(b1.Item1, b2.Item1, deltaTime));
                }
            }
        }

        private static bool IsColliding(BoundingBox b1, BoundingBox b2)
        {
            return b1.Contains(b2);
        }
    }
}
