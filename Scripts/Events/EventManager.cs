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
            Debug.Log($"Adding listener to : {triggerEvent} from {listener.Target.ToString()}");
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

                if (eventCache.ContainsKey(triggerEvent))
                {
                    Debug.Log($"Object in Cache for {triggerEvent}");
                    //listener(eventCache[triggerEvent]);
                }
            }

        }

        public static void TriggerEvent(Events eventToTrigger, object package, bool allowCaching = true)
        {
            Instance.TriggerListeners(eventToTrigger, package, allowCaching);
        }

        private void TriggerListeners(Events eventToTrigger, object package, bool allowCaching = true)
        {
            lock (event_lock)
            {
                OnEventTrigger thisEvent;
                if (listeners.TryGetValue(eventToTrigger, out thisEvent))
                {
                    thisEvent?.Invoke(package);
                    foreach(var d in thisEvent.GetInvocationList())
                    {
                        Debug.Log("Invoke on: " + d.Target.ToString());
                    }
                }
                if(!allowCaching) return;

                if (eventCache.ContainsKey(eventToTrigger))
                {
                    Debug.Log($"Updating Event cache: {eventToTrigger}, {package}");
                    eventCache[eventToTrigger] = package;
                }
                else
                {
                    Debug.Log($"Adding Event to cache: {eventToTrigger}, {package}");
                    eventCache.Add(eventToTrigger, package);
                }
            }

        }
    }

    public enum Events
    {
        None,
        WorldReady,
        ChangeScene,
        EscapeMenuToggle,
        SignsPlaced,
        SaveSignData,
        LoadScene,
        SceneActive,
        LoadSignData,
        SelectSignObject,
        
    }
}

