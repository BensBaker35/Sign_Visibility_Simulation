using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Events
{
    public class EventManager
    {
        public delegate void OnEventTrigger(object package);
        private Dictionary<Events, OnEventTrigger> listeners;
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
            }

        }

        public static void TriggerEvent(Events eventToTrigger, object package)
        {
            Instance.TriggerListeners(eventToTrigger, package);
        }

        private void TriggerListeners(Events eventToTrigger, object package)
        {
            lock (event_lock)
            {
                OnEventTrigger thisEvent;
                if (listeners.TryGetValue(eventToTrigger, out thisEvent))
                {
                    thisEvent?.Invoke(package);
                    // foreach(var d in thisEvent.GetInvocationList())
                    // {
                    //     Debug.Log("Invoke on: " + d.Target.ToString());
                    // }
                }
            }

        }

        public static void RemoveListener(Events ev, OnEventTrigger toRemove)
        {
            Instance.Remove(ev, toRemove);
        }

        private void Remove(Events ev, OnEventTrigger toRemove)
        {
            lock (event_lock)
            {
                if(listeners.ContainsKey(ev))
                {
                    listeners[ev] -= toRemove;
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

