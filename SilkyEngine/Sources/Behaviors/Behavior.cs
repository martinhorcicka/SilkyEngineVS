using System.Collections.Generic;
using Silk.NET.Windowing.Common;
using SilkyEngine.Sources.Entities;
using SilkyEngine.Sources.Interfaces;

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

        protected abstract void OnUpdate(double deltaTime);

        public interface IBindable
        {
        }
    }
}