using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Tools;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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
        private Func<float, float, float> heightMap;

        public BRandomWalk(IWindow window, float sleepTime = 2, float walkingSpeed = 4, Func<float, float, float> heightMap = null) : base(window)
        {
            this.sleepTime = sleepTime;
            this.walkingSpeed = walkingSpeed;
            this.heightMap = heightMap;

            sleepTimer = 0;
        }

        protected override void OnUpdate(double deltaTime)
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

                foreach (var e in entities)
                    e.Translate(Vector3.Zero);
            }
            else
            {
                Vector3 dPos = new Vector3(currentDirection.X, 0, currentDirection.Y);
                foreach (var e in entities)
                {
                    var prevHeight = heightMap?.Invoke(e.Position.X, e.Position.Z) ?? 0;
                    e.Translate(dPos * speed);
                    var currentHeight = heightMap?.Invoke(e.Position.X, e.Position.Z) ?? 0;
                    e.SetHeight(e.Position.Y + currentHeight - prevHeight);
                }
                walkTimer -= (float)deltaTime;
            }
        }
    }
}
