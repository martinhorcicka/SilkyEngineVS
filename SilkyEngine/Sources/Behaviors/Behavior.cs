using System.Collections.Generic;
using Silk.NET.Windowing;
using SilkyEngine.Sources.Entities;

namespace SilkyEngine.Sources.Behaviors
{
    public abstract class Behavior
    {
        public static Behavior DoNothing = null;
        protected List<Entity> entities = new List<Entity>();
        public Behavior(IWindow window)
        {
            window.Update += OnUpdate;
        }

        public virtual void SubscribeEntity(Entity entity)
        {
            if (entities.Contains(entity))
                return;

            entities.Add(entity);
        }

        public abstract void OnUpdate(double deltaTime);
    }
}