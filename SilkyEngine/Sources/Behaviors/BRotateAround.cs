using System.Numerics;
using Silk.NET.Windowing;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Tools;

namespace SilkyEngine.Sources.Behaviors
{
    public class BRotateAround : Behavior
    {
        private Vector3 point;
        private Vector3 axis;
        private float speed;

        public BRotateAround(IWindow window, Vector3 point, Vector3 axis, float speed)
            : base(window)
        {
            this.point = point;
            this.axis = axis;
            this.speed = speed;
        }

        public override void OnUpdate(double deltaTime)
        {
            foreach (var e in entities)
            {
                RotateEntity(e, speed * (float)deltaTime);
            }
        }

        private void RotateEntity(Entity e, float angle)
        {
            var rotMat = Matrix4x4.CreateFromAxisAngle(axis, angle);
            Vector3 R = e.Position - point;
            Vector3 rotR = Computation.MatMul(rotMat, R);
            e.DeltaPosition += rotR - R;
        }
    }
}