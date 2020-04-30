using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Physics
{
    public class BoundingBox : IBoundingVolume
    {
        public static BoundingBox Default { get; } = null;
        public static BoundingBox None { get; } = new BoundingBox(Vector3.Zero, Vector3.UnitY, 0, 0);
        private static List<Vector3> DefaultVertices { get; } = new List<Vector3>(9)
        {
            new Vector3( 0, 0, 0) * 0.5f,
            new Vector3(-1,-1,-1) * 0.5f,
            new Vector3(-1,-1, 1) * 0.5f,
            new Vector3(-1, 1,-1) * 0.5f,
            new Vector3(-1, 1, 1) * 0.5f,
            new Vector3( 1,-1,-1) * 0.5f,
            new Vector3( 1,-1, 1) * 0.5f,
            new Vector3( 1, 1,-1) * 0.5f,
            new Vector3( 1, 1, 1) * 0.5f,
        };

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
            var box = new BoundingBox(entity.Center, entity.Up, entity.Rotation.Y, entity.Scale * 2);
            CollisionDetection.Add(entity, box);
            return box;
        }

        public List<Vector3> MakeVertices()
        {
            var output = new List<Vector3>(9);
            foreach (var v in DefaultVertices)
            {
                output.Add(sideLength * v + center);
            }
            return output;
        }

        public bool Contains(IBoundingVolume volume) => Contains((BoundingBox)volume);

        private bool Contains(BoundingBox box)
        {
            var vertices = box.MakeVertices();
            foreach (var v in vertices)
                if (Contains(v)) return true;

            return false;
        }

        private bool Contains(Vector3 point)
        {
            if (MathF.Abs(point.X - center.X) > sideLength / 2) return false;
            if (MathF.Abs(point.Y - center.Y) > sideLength / 2) return false;
            if (MathF.Abs(point.Z - center.Z) > sideLength / 2) return false;

            return true;
        }
    }
}
