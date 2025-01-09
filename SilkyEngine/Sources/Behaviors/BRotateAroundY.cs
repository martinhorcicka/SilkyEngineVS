
using Silk.NET.Windowing;

namespace SilkyEngine.Sources.Behaviors
{
    public class BRotateAroundY : Behavior
    {
        private float speed;

        public BRotateAroundY(IWindow window, float speed) : base(window)
        {
            this.speed = speed;
        }

        public override void OnUpdate(double deltaTime)
        {
            foreach (var e in entities)
                e.RotateY(speed * (float)deltaTime);
        }
    }
}