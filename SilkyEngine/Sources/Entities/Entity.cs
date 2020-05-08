using System.Collections.Generic;
using System.Numerics;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;


namespace SilkyEngine.Sources.Entities
{
    public abstract class Entity
    {
        protected World world;
        protected BoundingVolume boundingVolume;
        protected TexturedModel texturedModel;
        protected Vector3 position;
        protected Vector3 rotation;
        protected Vector3 dimensions;
        protected float scale;

        public TexturedModel TexturedModel => texturedModel;
        public Vector3 Center => texturedModel.Center * scale + position;
        public Vector3 Position => position;
        public Vector3 Rotation => rotation;
        public Vector3 Up => Vector3.UnitY;
        public Vector3 Dimensions => dimensions;
        public float Scale => scale;
        public float Mass { get; set; } = 0;
        public Vector3 DeltaPosition { get; set; } = Vector3.Zero;

        public Entity(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
        {
            this.world = world;
            this.texturedModel = texturedModel;
            this.position = position;
            this.rotation = rotation;
            this.dimensions = dimensions;
            this.scale = scale;
            behavior?.SubscribeEntity(this);

            if (boundingVolume is BoundingBox && boundingVolume == BoundingBox.None)
                this.boundingVolume = BoundingBox.None;
            else if (boundingVolume is BoundingSphere && boundingVolume == BoundingSphere.None)
                this.boundingVolume = BoundingSphere.None;
            else this.boundingVolume = boundingVolume.FromEntity(this);

        }
        public Entity(World world, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(world, boundingVolume, behavior, texturedModel, position, rotation, scale, scale * Vector3.One)
        { }


        public void OnMove() => Translate(DeltaPosition);

        protected virtual void Translate(Vector3 dp)
        {
            boundingVolume.Translate(dp);
            position += dp;
        }

        public virtual void RotateX(float angle)
        {
            boundingVolume.RotateX(angle);
            rotation.X += angle;
        }

        public virtual void RotateY(float angle)
        {
            boundingVolume.RotateY(angle);
            rotation.Y += angle;
        }

        public virtual void RotateZ(float angle)
        {
            boundingVolume.RotateZ(angle);
            rotation.Z += angle;
        }

        public virtual void RotateAroundAxis(Vector3 axis, float angle) => rotation += axis * angle;

        public abstract void Collision(List<EntityCollisionInfo> cInfos);
    }
}