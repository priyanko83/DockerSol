using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public static class DomainEventsManager
    {
        [ThreadStatic]
        private static List<Delegate> actions;

        static DomainEventsManager()
        {
            //TODO: Replace with Unity
            //Container = StructureMap.ObjectFactory.Container;
        }

        //TODO: Replace with Unity
        //public static IContainer Container { get; set; }
        public static void Register<T>(Action<T> callback) where T : DomainEvent
        {
            if (actions == null)
            {
                actions = new List<Delegate>();
            }
            actions.Add(callback);
        }

        public static void ClearCallbacks()
        {
            actions = null;
        }

        public static void Raise<T>(T args) where T : DomainEvent
        {
            //TODO: Replace with Unity
            //foreach (var handler in Container.GetAllInstances<IHandle<T>>())
            //{
            //    handler.Handle(args);
            //}

            if (actions != null)
            {
                foreach (var action in actions)
                {
                    if (action is Action<T>)
                    {
                        ((Action<T>)action)(args);
                    }
                }
            }
        }
    }
}
