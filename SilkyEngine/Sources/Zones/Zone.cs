using SilkyEngine.Sources.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilkyEngine.Sources.Zones
{
    public abstract class Zone
    {
        public abstract bool IsInside(float x, float z);

    }
}
