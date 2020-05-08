using MathNet.Numerics.Optimization.LineSearch;
using MathNet.Numerics.Providers.LinearAlgebra;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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
            foreach (var cInfo in collisionInfos)
            {
                var normal = cInfo.Normal;
                var mag = Vector3.Dot(normal, DeltaPosition);

                switch (cInfo.Entity)
                {
                    case Terrain terrain:
                        IsOnGround = true;
                        VerticalSpeed = 0;

                        if (mag > 0)
                            DeltaPosition -= mag * normal;

                        break;
                    case Obstacle obstacle:
                        DeltaPosition += obstacle?.DeltaPosition ?? Vector3.Zero;

                        float magY = Vector3.Dot(normal, Vector3.UnitY);
                        if (magY < 0)
                        {
                            VerticalSpeed = 0;
                            IsOnGround = true;
                        }
                        else if (VerticalSpeed > 0 && magY > 0.8f)
                        {
                            VerticalSpeed = 0;
                        }

                        if (mag > 0)
                            DeltaPosition -= mag * normal;
                        break;

                    case Movable movable:
                        if (mag < 0) return;

                        float massFrac = Mass / (Mass + movable.Mass);
                        DeltaPosition -= mag * normal * (1 - massFrac);
                        movable.DeltaPosition += mag * normal * massFrac;
                        break;
                }
            }
        }
    }
}
