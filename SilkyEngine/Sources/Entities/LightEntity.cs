using System;
using System.Numerics;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Graphics.Structs;
using SilkyEngine.Sources.Physics.Collisions;

namespace SilkyEngine.Sources.Entities
{
    public class LightEntity : Entity
    {
        public event Action LightMoved;
        private LightStruct lightStruct;

        public LightEntity(World world, BoundingVolume boundingVolume, Behavior behavior, LightStruct lightStruct, TexturedModel texturedModel, float scale)
            : base(world, boundingVolume, behavior, texturedModel, lightStruct.Position, Vector3.Zero, scale)
        {
            this.lightStruct = lightStruct;
        }

        protected override void Translate(Vector3 dp)
        {
            base.Translate(dp);
            lightStruct.Position = position;
            LightMoved?.Invoke();
        }

        public LightStruct GetLightStruct() => lightStruct;
        public string GetLightStructName() => lightStruct.Name;

        public override void Collision(CollisionInfo cInfo)
        {
        }
    }
}