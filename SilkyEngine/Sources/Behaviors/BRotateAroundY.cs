using Silk.NET.Windowing.Common;

namespace SilkyEngine.Sources.Behaviors
{
    public class BRotateAroundY : Behavior
    {
        private float speed;

        public BRotateAroundY(IWindow window, float speed) : base(window)
        {
            this.speed = speed;
        }

        protected override void OnUpdate(double deltaTime)
        {
            foreach (var e in entities)
                e.RotateY(speed * (float)deltaTime);
        }
    }
}