using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Events
{
    public class EventManager
    {
        public delegate void OnEventTrigger(object package);
        private Dictionary<Events, OnEventTrigger> listeners;
        private Dictionary<Events, object> eventCache;
        private static EventManager instance;
        private static EventManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new EventManager();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        private static object event_lock = new object();

        private EventManager()
        {
            listeners = new();
            eventCache = new();
        }

        public static void Listen(Events triggerEvent, OnEventTrigger listener)
        {
            Instance.AddListener(triggerEvent, listener);
        }

        private void AddListener(Events triggerEvent, OnEventTrigger listener)
        {
            lock (event_lock)
            {
                OnEventTrigger thisEvent;
                if (listeners.TryGetValue(triggerEvent, out thisEvent))
                {
                    thisEvent += listener;
                    listeners[triggerEvent] = thisEvent;
                }
                else
                {
                    thisEvent += listener;
                    listeners.Add(triggerEvent, thisEvent);
                }

                if(eventCache.ContainsKey(triggerEvent))
                {
                    listener(eventCache[triggerEvent]);
                }
            }

        }

        public static void TriggerEvent(Events eventToTrigger, object package)
        {
            Instance.TriggerListeners(eventToTrigger, package);
        }

        private void TriggerListeners(Events eventToTrigger, object package)
        {
            OnEventTrigger thisEvent;
            if (listeners.TryGetValue(eventToTrigger, out thisEvent))
            {
                thisEvent?.Invoke(package);
            }

            if(eventCache.ContainsKey(eventToTrigger))
            {
                eventCache[eventToTrigger] = package;
            } 
            else
            {
                eventCache.Add(eventToTrigger, package);
            }
        }
    }

    public enum Events
    {
        None,
        WorldReady,
        ChangeScene,
        EscapeMenuToggle,
        Save,
        SaveAs,
        Load,
        SignsPlaced,
    }
}

