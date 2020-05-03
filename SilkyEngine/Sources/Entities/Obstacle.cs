using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Obstacle : Entity
    {
        public Obstacle(BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : base(boundingVolume, behavior, texturedModel, position, rotation, scale)
        { }

        public Obstacle(BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(boundingVolume, behavior, texturedModel, position, rotation, scale, dimensions)
        { }

        public override void Collision(CollisionInfo cInfo)
        {
            if (!(cInfo.Target is Player p)) return;
            float distance = p.MovementSpeed * (float)cInfo.DeltaTime;
            Vector3 dPos = cInfo.Normal;
            if (dPos.Y == 1)
            {
                p.IsInAir = false;
                p.VerticalSpeed = 0;
                dPos.Y = 0;
            }
            else if (dPos.Y == -1)
            {
                p.VerticalSpeed = 0;
            }

            p.Translate(dPos * distance + MovedBy);
        }
    }
}
