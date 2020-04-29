using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics
{
    public class BoundingBox
    {
        public static BoundingBox Default { get; } = null;
        public static BoundingBox None { get; } = new BoundingBox(Vector3.Zero, Vector3.UnitY, 0, 0);

        private Vector3 center;
        private Vector3 up;
        private float rotation;
        private float sideLength;

        private BoundingBox(Vector3 center, Vector3 up, float rotation, float sideLength)
        {
            this.center = center;
            this.up = up;
            this.rotation = rotation;
            this.sideLength = sideLength;
        }

        public Vector3 Center
        {
            get { return center; }
            set { center = value; }
        }

        public void Translate(Vector3 dp) => center += dp;
        public void SetHeight(float newHeight) => center.Y = newHeight;


        public static BoundingBox FromEntity(Entity entity)
        {
            var box = new BoundingBox(entity.Center, entity.Up, entity.Rotation.Y, entity.Scale);
            CollisionDetection.Add(entity, box);
            return box;
        }

        public static float CenterDistance(BoundingBox b1, BoundingBox b2)
        {
            return (b1.center - b2.center).Length();
        }
    }
}
