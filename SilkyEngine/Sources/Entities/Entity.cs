using System;
using System.Numerics;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;

namespace SilkyEngine.Sources.Entities
{
    public class Entity
    {
        public static Entity Empty = null;
        protected TexturedModel texturedModel;
        protected Vector3 position;
        protected Vector3 rotation;
        protected float scale;

        public Entity(Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
        {
            behavior?.SubscribeEntity(this);
            this.texturedModel = texturedModel;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public TexturedModel TexturedModel => texturedModel;
        public void SetHeight(float newHeight) => position.Y = newHeight;
        public Vector3 Position => position;
        public Vector3 Rotation => rotation;
        public float Scale => scale;

        internal virtual void Translate(Vector3 dp) => position += dp;
        internal virtual void RotateX(float angle) => rotation.X += angle;
        internal virtual void RotateY(float angle) => rotation.Y += angle;
        internal virtual void RotateZ(float angle) => rotation.Z += angle;
        internal virtual void RotateAroundAxis(Vector3 axis, float angle) => rotation += axis * angle;

    }
}