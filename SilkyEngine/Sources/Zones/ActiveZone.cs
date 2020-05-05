using SilkyEngine.Sources.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilkyEngine.Sources.Zones
{
    public abstract class ActiveZone : Zone
    {
        protected List<Entity> entities = new List<Entity>();
        public void OnUpdate(double deltaTime)
        {
            UpdateMethod(deltaTime);
            entities.Clear();
        }
        protected abstract void UpdateMethod(double deltaTime);
        public void AddEntity(Entity e)
        {
            entities.Add(e);
        }
    }
}
