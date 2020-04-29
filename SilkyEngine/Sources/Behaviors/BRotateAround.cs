using System;
using System.Numerics;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Tools;

namespace SilkyEngine.Sources.Behaviors
{
    public class BRotateAround : Behavior
    {
        private Vector3 point;
        private Vector3 axis;
        private float speed;
        private Func<float, float, float> heightMap;

        public BRotateAround(IWindow window, Vector3 point, Vector3 axis, float speed, Func<float, float, float> heightMap = null)
            : base(window)
        {
            this.point = point;
            this.axis = axis;
            this.speed = speed;
            this.heightMap = heightMap;
        }
        protected override void OnUpdate(double deltaTime)
        {
            foreach (var e in entities)
            {
                RotateEntity(e, speed * (float)deltaTime);
                e.SetHeight(2 + heightMap?.Invoke(e.Position.X, e.Position.Z) ?? 0);
            }
        }

        private void RotateEntity(Entity e, float angle)
        {
            var rotMat = Matrix4x4.CreateFromAxisAngle(axis, angle);
            Vector3 R = e.Position - point;
            Vector3 rotR = Computation.MatMul(rotMat, R);
            e.Translate(rotR - R);
        }
    }
}