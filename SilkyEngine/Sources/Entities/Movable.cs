using MathNet.Numerics.Optimization.LineSearch;
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
        public float RelMass { get; set; }
        public bool IsOnGround { get; protected set; }
        protected float Gravity { get; } = 20f;

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

        public virtual void OnUpdate(double deltaTime)
        {
            Translate(Vector3.UnitY * VerticalSpeed * (float)deltaTime);
            VerticalSpeed -= Gravity * (float)deltaTime;

            float terraintHeight = world.GetHeight(Position.X, Position.Z);
            if (Position.Y < terraintHeight)
            {
                IsOnGround = true;
                VerticalSpeed = 0;
                SetHeight(terraintHeight);
            }
        }

        public override void Collision(CollisionInfo cInfo)
        {
            if (!(cInfo.Target is Player || cInfo.Target is Movable)) return;
            if (cInfo.Target is Movable m)
            {
                m.Translate(cInfo.Normal * m.CurrentSpeed * (float)cInfo.DeltaTime);
                RelMass = Mass + m.RelMass;
            }
            if (cInfo.Target is Player p)
            {

                float deltaTime = (float)cInfo.DeltaTime;
                float distance = p.MovementSpeed * deltaTime;
                Vector3 dPos = cInfo.Normal;
                float massFrac = RelMass / (RelMass + p.Mass);

                p.Translate(dPos * distance * massFrac);
                Translate(-dPos * distance * (1 - massFrac));
                CurrentSpeed = MovedBy.Length() / deltaTime;
                RelMass = Mass;
            }
        }
    }
}
