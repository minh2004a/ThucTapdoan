using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem.Events
{
    public class EventDispatcher : MonoBehaviour
    {
        private static EventDispatcher _instance;
        public static EventDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("EventDispatcher");
                    _instance = go.AddComponent<EventDispatcher>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private Dictionary<Type, Delegate> eventRegistry = new Dictionary<Type, Delegate>();

        public void Subscribe<T>(Action<T> handler) where T : class
        {
            var eventType = typeof(T);

            if (eventRegistry.ContainsKey(eventType))
            {
                eventRegistry[eventType] = Delegate.Combine(eventRegistry[eventType], handler);
            }
            else
            {
                eventRegistry[eventType] = handler;
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            var eventType = typeof(T);

            if (eventRegistry.ContainsKey(eventType))
            {
                eventRegistry[eventType] = Delegate.Remove(eventRegistry[eventType], handler);

                if (eventRegistry[eventType] == null)
                {
                    eventRegistry.Remove(eventType);
                }
            }
        }

        public void Dispatch<T>(T eventData) where T : class
        {
            var eventType = typeof(T);

            if (eventRegistry.TryGetValue(eventType, out Delegate del))
            {
                var callback = del as Action<T>;
                callback?.Invoke(eventData);
            }
        }

        public void ClearAll()
        {
            eventRegistry.Clear();
        }

        private void OnDestroy()
        {
            ClearAll();
        }
    }
}
