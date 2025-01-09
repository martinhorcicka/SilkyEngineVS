using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SilkyEngine.Sources.Entities
{
    public class Movable : Entity
    {
        public virtual float CurrentSpeed { get; set; }
        public float VerticalSpeed { get; set; } = 0;
        public bool IsOnGround { get; protected set; }
        public bool GravityOn { get; set; } = true;

        public Movable(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(world, boundingVolume, behavior, texturedModel, position, rotation, scale, scale * Vector3.One)
        {
        }

        public Movable(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(world, boundingVolume, behavior, texturedModel, position, rotation, scale, dimensions)
        {
            this.world = world;
            Mass = 1f;
        }

        protected override void Translate(Vector3 dp)
        {
            if (VerticalSpeed < -5f) IsOnGround = false;
            base.Translate(dp);
        }

        public override void Collision(List<EntityCollisionInfo> collisionInfos)
        {
            float MassSum = collisionInfos.FindAll((cI) => cI.Entity is Movable).Sum((cI) => cI.Entity.Mass) + Mass;
            List<EntityCollisionInfo> newCollisions = new List<EntityCollisionInfo>();
            Vector3 addedDeltaPosition = Vector3.Zero;
            float mag = 0f;

            foreach (var cInfo in collisionInfos)
            {
                var normal = cInfo.Normal;

                switch (cInfo.Entity)
                {
                    case Terrain terrain:
                        IsOnGround = true;
                        VerticalSpeed = 0;
                        break;

                    case Obstacle obstacle:
                        DeltaPosition += obstacle?.DeltaPosition ?? Vector3.Zero;
                        mag = Vector3.Dot(DeltaPosition, normal);
                        if (mag < 0) continue;
                        DeltaPosition -= mag * normal;

                        if (normal.Y < 0)
                        {
                            VerticalSpeed = 0;
                            IsOnGround = true;
                        }
                        else if (VerticalSpeed > 0 && normal.Y > 0.8f)
                        {
                            VerticalSpeed = 0;
                        }
                        break;

                    case Movable movable:
                        mag = Vector3.Dot(normal, movable.DeltaPosition);
                        if (mag > 0) continue;

                        float massFrac = movable.Mass / MassSum;
                        addedDeltaPosition += mag * normal * massFrac;
                        movable.DeltaPosition -= mag * normal * (1 - massFrac);
                        break;
                }
            }
            DeltaPosition += addedDeltaPosition;
        }
    }
}
