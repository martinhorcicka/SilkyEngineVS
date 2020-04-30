using System;
using System.Numerics;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Interfaces;
using SilkyEngine.Sources.Physics;

namespace SilkyEngine.Sources.Entities
{
    public class Player : Entity
    {
        private Vector3 focus;
        public Player(BoundingBox boundingBox, IPlayerController controls, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : base(boundingBox, (Behavior)controls, texturedModel, position, rotation, scale)
        {
            controls.SubscribePlayer(this);
            focus = 0.25f * Vector3.UnitY;
        }

        public Vector3 Focus => Center + focus;
        public Vector3 Right => Vector3.Cross(Front, Up);
        public float JumpPower => 8f;
        public float Thickness => 0.5f;

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

            rotation.Y = MathF.Atan2(fr.Y, fr.X);
        }
    }
}