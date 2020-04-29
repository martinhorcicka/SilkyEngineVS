using System;
using System.Collections.Generic;
using System.Numerics;
using SilkyEngine.Sources.Behaviors;
using SilkyEngine.Sources.Graphics;
using SilkyEngine.Sources.Graphics.Structs;
using SilkyEngine.Sources.Physics;

namespace SilkyEngine.Sources.Entities
{
    public class LightEntity : Entity
    {
        public event Action LightMoved;
        private LightStruct lightStruct;
        public LightEntity(BoundingBox boundingBox, Behavior behavior, LightStruct lightStruct, TexturedModel texturedModel, float scale)
            : base(boundingBox, behavior, texturedModel, lightStruct.position, Vector3.Zero, scale)
        {
            this.lightStruct = lightStruct;
        }

        public override void Translate(Vector3 dp)
        {
            base.Translate(dp);
            lightStruct.position = position;
            LightMoved?.Invoke();
        }

        public LightStruct GetLightStruct() => lightStruct;
        public string GetLightStructName() => lightStruct.name;
    }
}