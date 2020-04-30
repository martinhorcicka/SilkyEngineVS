using System;
using System.Collections.Generic;
using System.Text;

namespace SilkyEngine.Sources.Interfaces
{
    public interface IBoundingVolume
    {
        public bool Contains(IBoundingVolume volume);
    }
}
