using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace SilkyEngine.Sources.Zones
{
    public class HotZone : ActiveZone
    {
        private RectangleF area;

        public HotZone(RectangleF area)
        {
            this.area = area;
        }

        public override bool IsInside(float x, float z)
        {
            return area.Contains(x, z);
        }

        protected override void UpdateMethod(double deltaTime)
        {
            foreach (var e in entities)
            {
                e.Translate(Vector3.UnitY * (float)deltaTime * 5);
            }
        }
    }
}
