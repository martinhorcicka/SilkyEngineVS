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
            Vector3 dPos = ToBoxNormal(p.Center - Center);
            if (dPos.Y == 1)
            {
                p.IsInAir = false;
                p.VerticalSpeed = 0;
                dPos.Y = 0;
            }
            else if(dPos.Y == -1)
            {
                p.VerticalSpeed = 0;
            }

            p.Translate(dPos * distance);
        }

        private Vector3 ToBoxNormal(Vector3 R)
        {
            float abs(float x) => MathF.Abs(x);
            int sgn(float x) => MathF.Sign(x);
            Vector3 nR = R / dimensions;
            float x = nR.X, y = nR.Y, z = nR.Z;
            int index = 0;
            if (abs(x) < abs(y))
            {
                index = 1;
                if (abs(y) < abs(z))
                    index = 2;
            }
            else if (abs(x) < abs(z))
                index = 2;

            if (index == 0) return sgn(x) * Vector3.UnitX;
            if (index == 1) return sgn(y) * Vector3.UnitY;
            if (index == 2) return sgn(z) * Vector3.UnitZ;

            return Vector3.UnitY;
        }
    }
}
