using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Tools;
using System;
using System.Numerics;

namespace SilkyEngine.Sources.Behaviors
{
    class BRandomWalk : Behavior
    {
        private const float MAX_DURATION = 1, MIN_DURATION = 0;
        Random generator = new Random();
        float walkTimer, sleepTimer;
        float sleepTime = 2;
        float walkingSpeed = 4;
        Vector2 currentDirection;

        public BRandomWalk(IWindow window, float sleepTime = 2, float walkingSpeed = 4) : base(window)
        {
            this.sleepTime = sleepTime;
            this.walkingSpeed = walkingSpeed;

            sleepTimer = 0;
        }

        public override void OnUpdate(double deltaTime)
        {
            if (sleepTimer > 0)
            {
                sleepTimer -= (float)deltaTime;
                return;
            }

            float speed = walkingSpeed * (float)deltaTime;
            if (walkTimer <= 0)
            {
                sleepTimer = sleepTime;
                walkTimer = (MAX_DURATION - MIN_DURATION) * (float)generator.NextDouble() + MIN_DURATION;
                currentDirection = Computation.RandomVector2(generator);
            }
            else
            {
                Vector3 dPos = new Vector3(currentDirection.X, 0, currentDirection.Y);
                foreach (var e in entities)
                {
                    e.DeltaPosition += dPos * speed;
                }
                walkTimer -= (float)deltaTime;
            }
        }
    }
}
