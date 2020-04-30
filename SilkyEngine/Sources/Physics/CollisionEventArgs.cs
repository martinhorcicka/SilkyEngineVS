using SilkyEngine.Sources.Entities;
using System;

namespace SilkyEngine.Sources.Physics
{
    public class CollisionEventArgs
    {
        private Entity entity1, entity2;
        public double DeltaTime { get; }

        public CollisionEventArgs(Entity e1, Entity e2, double deltaTime)
        {
            this.entity1 = e1;
            this.entity2 = e2;
            DeltaTime = deltaTime;
        }

        private bool isPlayer()
        {
            return (entity1 is Player || entity2 is Player);
        }

        public bool Unwrap(out Player p, out Entity e)
        {
            p = null; e = null;
            if (!isPlayer()) return false;

            if (entity1 is Player)
            {
                p = (Player)entity1;
                e = entity2;
            }
            else
            {
                p = (Player)entity2;
                e = entity1;
            }

            return true;
        }
    }
}