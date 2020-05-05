using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilkyEngine.Sources.Zones
{
    public abstract class Zone
    {
        public abstract bool IsInside(float x, float z);
        public virtual bool IsInside(Entity entity) => IsInside(entity.Position.X, entity.Position.Z);

    }
}
