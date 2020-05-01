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
        protected Vector3 dimensions;
        protected float scale;

        public Entity(BoundingBox boundingBox, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
        {
            this.texturedModel = texturedModel;
            this.position = position;
            this.rotation = rotation;
            this.dimensions = dimensions;
            this.scale = scale;
            behavior?.SubscribeEntity(this);
            this.boundingBox = (boundingBox == BoundingBox.Default) ? BoundingBox.FromEntity(this) : BoundingBox.None;
        }
        public Entity(BoundingBox boundingBox, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(boundingBox, behavior, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }

        public TexturedModel TexturedModel => texturedModel;
        public Vector3 Center => texturedModel.Center * scale + position;
        public void SetHeight(float newHeight)
        {
            movedBy += (newHeight - position.Y) * Vector3.UnitY;
            boundingBox.SetHeight(newHeight + texturedModel.Center.Y * scale);
            position.Y = newHeight;
        }
        public Vector3 Position
        {
            get { return position; }
        }
        public Vector3 Rotation => rotation;
        public Vector3 Up => Vector3.UnitY;
        public Vector3 Dimensions => dimensions;
        public float Scale => scale;

        private Vector3 movedBy = Vector3.Zero;
        public Vector3 MovedBy => movedBy;

        public virtual void Translate(Vector3 dp)
        {
            movedBy = dp;
            boundingBox.Translate(dp);
            position += dp;
        }
        public virtual void RotateX(float angle)
        {
            boundingBox.RotateX(angle);
            rotation.X += angle;
        }
        public virtual void RotateY(float angle)
        {
            boundingBox.RotateY(angle);
            rotation.Y += angle;
        }
        public virtual void RotateZ(float angle)
        {
            boundingBox.RotateZ(angle);
            rotation.Z += angle;
        }
        public virtual void RotateAroundAxis(Vector3 axis, float angle) => rotation += axis * angle;

    }
}