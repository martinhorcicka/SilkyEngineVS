using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SilkyEngine.Sources.Zones
{
    public class HomeZone : Zone
    {
        private RectangleF area;

        public HomeZone(RectangleF area)
        {
            this.area = area;
        }

        public override bool IsInside(float x, float z)
        {
            return area.Contains(x, z);
        }

        public bool IsInside(Entity e)
        {
            return IsInside(e.Position.X, e.Position.Z);
        }
    }
}
