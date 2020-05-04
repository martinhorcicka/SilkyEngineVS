using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Physics.Collisions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Entities
{
    public class Movable : Entity
    {
        public float CurrentSpeed { get; private set; }
        public bool IsInAir { get; private set; }
        public float VerticalSpeed { get; private set; }
        private const float gravity = 20f;

        protected List<Terrain> terrains;

        public Movable(List<Terrain> terrains, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale)
            : this(terrains, boundingVolume, behavior, texturedModel, position, rotation, scale, scale * Vector3.One)
        {
        }

        public Movable(List<Terrain> terrains, BoundingVolume boundingVolume, Behavior behavior, TexturedModel texturedModel, Vector3 position, Vector3 rotation, float scale, Vector3 dimensions)
            : base(boundingVolume, behavior, texturedModel, position, rotation, scale, dimensions)
        {
            this.terrains = terrains;
            Mass = 1f;
        }

        public override void Collision(CollisionInfo cInfo)
        {
            if (!(cInfo.Target is Player p)) return;

            IsInAir = true;
            float deltaTime = (float)cInfo.DeltaTime;
            float distance = p.MovementSpeed * deltaTime;
            Vector3 dPos = cInfo.Normal;
            float massFrac = Mass / (Mass + p.Mass);

            if (IsInAir)
            {
                VerticalSpeed -= gravity * deltaTime;
                Translate(Vector3.UnitY * VerticalSpeed * deltaTime);
            }

            float terraintHeight = GetHeightFromTerrains();
            if (position.Y < terraintHeight)
            {
                IsInAir = false;
                VerticalSpeed = 0;
                SetHeight(terraintHeight);
            }

            p.Translate(dPos * distance * massFrac);
            Translate(-dPos * distance * (1 - massFrac));
            CurrentSpeed = MovedBy.Length() / deltaTime;
        }

        private float GetHeightFromTerrains()
        {
            foreach (var t in terrains)
            {
                if (t.TryGetHeigt(position.X, position.Z, out float height))
                    return height;
            }
            return 0;
        }
    }
}
