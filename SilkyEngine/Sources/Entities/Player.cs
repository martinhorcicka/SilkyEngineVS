using System;
using System.Numerics;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Physics.Collisions;

namespace SilkyEngine.Sources.Entities
{
    public class Player : Entity
    {
        public event Action<Vector3> Move;

        public float MovementSpeed => 10f;
        public float VerticalSpeed { get; set; } = 0;
        public bool IsInAir { get; set; } = true;
        public bool Collided { get; set; }

        private Vector3 focus;
        public Player(BoundingVolume boundingVolume, IPlayerController controls, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(boundingVolume, (Behavior)controls, texturedModel, position, rotation, scale, dimensions)
        {
            controls.SubscribePlayer(this);
            focus = 0.25f * Vector3.UnitY;
            Mass = 1f;
        }

        public Player(BoundingVolume boundingVolume, IPlayerController controls, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(boundingVolume, controls, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }

        public Vector3 Focus => Center + focus;
        public Vector3 Right => Vector3.Cross(Front, Up);
        public float JumpPower => 10f;
        public float Thickness => 0.5f;

        public override void Translate(Vector3 dp)
        {
            base.Translate(dp);
            Move?.Invoke(dp);
        }

        public Vector3 Front
        {
            get
            {
                Vector3 retVec;
                retVec.X = MathF.Sin(rotation.Y);
                retVec.Y = 0;
                retVec.Z = MathF.Cos(rotation.Y);

                return retVec;
            }
        }

        public void SnapToFront(Vector3 cameraFront)
        {
            Vector2 fr = new Vector2(cameraFront.Z, cameraFront.X);
            float angle = MathF.Atan2(fr.Y, fr.X) - rotation.Y;
            RotateY(angle);
        }

        public override void Collision(CollisionInfo cInfo)
        {
            Collided = true;
        }
    }
}