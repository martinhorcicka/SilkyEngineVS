using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Obstacle : Entity
    {
        public static Obstacle Empty { get; } = null;
        public Obstacle(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(world, boundingVolume, behavior, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }

        public Obstacle(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(world, boundingVolume, behavior, texturedModel, position, rotation, scale, dimensions)
        { }

        public override void Collision(List<EntityCollisionInfo> collisionInfos)
        {
            foreach (var cInfo in collisionInfos)
            {
                var normal = cInfo.Normal;

                switch (cInfo.Entity)
                {
                    case Movable movable:
                        var mag = Vector3.Dot(normal, movable.DeltaPosition);

                        if (mag > 0) continue;

                        movable.DeltaPosition -= mag * normal;

                        break;
                }
            }
        }
    }
}

