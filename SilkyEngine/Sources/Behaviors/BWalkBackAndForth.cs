using Silk.NET.Windowing;
using System.Numerics;

namespace SilkyEngine.Sources.Behaviors
{
    class BWalkBackAndForth : Behavior
    {
        float speed;
        private Vector3 offset;
        private Vector3 direction;
        private float remaining;

        public BWalkBackAndForth(IWindow window, float speed, Vector3 offset)
            : base(window)
        {
            this.speed = speed;
            this.offset = offset;
            remaining = offset.Length();
            direction = offset / remaining;
        }

        public override void OnUpdate(double deltaTime)
        {
            if (remaining < 0)
            {
                remaining = offset.Length();
                direction *= -1;
                return;
            }

            float dDistance = speed * (float)deltaTime;
            remaining -= dDistance;

            foreach (var e in entities)
                e.DeltaPosition += dDistance * direction;
        }
    }
}
