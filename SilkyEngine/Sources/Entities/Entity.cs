using System;
using System.Numerics;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics;

namespace SilkyEngine.Sources.Entities
{
    public class Entity
    {
        public static Entity Empty = null;
        protected BoundingBox boundingBox;
        protected TexturedModel texturedModel;
        protected Vector3 position;
        protected Vector3 rotation;
        protected float scale;

        public Entity(BoundingBox boundingBox, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
        {
            this.texturedModel = texturedModel;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            behavior?.SubscribeEntity(this);
            this.boundingBox = (boundingBox == BoundingBox.Default) ? BoundingBox.FromEntity(this) : BoundingBox.None;
        }

        public TexturedModel TexturedModel => texturedModel;
        public Vector3 Center => texturedModel.Center * scale + position;
        public void SetHeight(float newHeight)
        {
            boundingBox.SetHeight(newHeight + texturedModel.Center.Y);
            position.Y = newHeight;
        }
        public Vector3 Position
        {
            get { return position; }
            set
            {
                boundingBox.Center = value;
                position = value;
            }
        }
        public Vector3 Rotation => rotation;
        public Vector3 Up => Vector3.UnitY;
        public float Scale => scale;

        public virtual void Translate(Vector3 dp)
        {
            boundingBox.Translate(dp);
            position += dp;
        }
        public virtual void RotateX(float angle) => rotation.X += angle;
        public virtual void RotateY(float angle) => rotation.Y += angle;
        public virtual void RotateZ(float angle) => rotation.Z += angle;
        public virtual void RotateAroundAxis(Vector3 axis, float angle) => rotation += axis * angle;

    }
}