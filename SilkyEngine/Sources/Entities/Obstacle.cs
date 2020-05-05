using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Obstacle : Entity
    {
        public Obstacle(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(world, boundingVolume, behavior, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }

        public Obstacle(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(world, boundingVolume, behavior, texturedModel, position, rotation, scale, dimensions)
        { }

        public override void Collision(CollisionInfo cInfo)
        {
            if (!(cInfo.Target is Movable m)) return;

            float distance = m.CurrentSpeed * (float)cInfo.DeltaTime;
            Vector3 dPos = cInfo.Normal;

            if (dPos.Y == 1f)
            {
                m.VerticalSpeed = 0;
                dPos.Y = 0;
            }
            else if (dPos.Y == -1f && m.VerticalSpeed > 0)
            {
                m.VerticalSpeed = 0;
            }

            m.Translate(dPos * distance + MovedBy);
        }
    }
}

