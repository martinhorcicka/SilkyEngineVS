using SilkyEngine.Sources.Entities;
using System;

namespace SilkyEngine.Sources.Physics
{
    public class CollisionEventArgs
    {
        private Tuple<Entity, BoundingBox> box1, box2;
        public double DeltaTime { get; }

        public CollisionEventArgs(Tuple<Entity, BoundingBox> box1, Tuple<Entity, BoundingBox> box2, double deltaTime)
        {
            this.box1 = box1;
            this.box2 = box2;
            DeltaTime = deltaTime;
        }

        private bool isPlayer()
        {
            return (box1.Item1 is Player || box2.Item1 is Player);
        }

        public Player GetPlayer()
        {
            if (!isPlayer()) return null;

            if (box1.Item1 is Player) return (Player)box1.Item1;

            return (Player)box2.Item1;
        }

        public Tuple<Player, Entity> Unwrap()
        {
            var p = GetPlayer();
            Entity e;
            if (box1.Item1 == p)
                e = box2.Item1;
            else
                e = box1.Item1;

            return Tuple.Create(p, e);
        }
    }
}