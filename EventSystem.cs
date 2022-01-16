using System;
using System.Collections.Generic;

namespace Eventing.EventSystem
{
    public class EventSystem
    {
        private readonly Dictionary<int, List<IObserver>> m_observers;
        private readonly Queue<GameEventData> m_gameEvents;
        private bool isProcessingEvent;

        private static EventSystem instance;
        private static EventSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventSystem();
                }
        
                return instance;
            }
        }

        private EventSystem()
        {
            m_observers  = new Dictionary<int, List<IObserver>>();
            m_gameEvents = new Queue<GameEventData>();
            isProcessingEvent = false;
        }

        ~EventSystem()
        {
            m_observers.Clear();
            m_gameEvents.Clear();
        }

        public static void Register(IObserver p_observer, params Type[] p_eventTypes)
        {
            Instance.RegisterInternal(p_observer, p_eventTypes);
        }

        private void RegisterInternal(IObserver p_observer, params Type[] p_eventTypes)
        {
            if (p_observer == null 
                || p_eventTypes == null 
                || p_eventTypes.Length == 0)
            {
                return;
            }

            for (int i = 0; i < p_eventTypes.Length; i++)
            {
                Type currentEventType = p_eventTypes[i];

                if (!currentEventType.IsEnum)
                {
                    continue;
                }

                int eventTypeId = currentEventType.GetHashCode();

                if (m_observers.TryGetValue(eventTypeId, out var observerList))
                {
                    if (!observerList.Contains(p_observer))
                    {
                        observerList.Add(p_observer);

                        m_observers[eventTypeId] = observerList;
                    }
                }
                else
                {
                    observerList = new List<IObserver>();
                    observerList.Add(p_observer);

                    m_observers[eventTypeId] = observerList;
                }
            }
        }

        public static void Unregister(IObserver p_observer, params Type[] p_eventTypes)
        {
            Instance.UnregisterInternal(p_observer, p_eventTypes);
        }

        private void UnregisterInternal(IObserver p_observer, params Type[] p_eventTypes)
        {
            if (p_observer == null
                || p_eventTypes == null
                || p_eventTypes.Length == 0)
            {
                return;
            }

            for (int i = 0; i < p_eventTypes.Length; i++)
            {
                Type currentEventType = p_eventTypes[i];

                if (!currentEventType.IsEnum)
                {
                    continue;
                }

                int eventTypeId = currentEventType.GetHashCode();

                if (!m_observers.ContainsKey(eventTypeId))
                {
                   return;
                }

                if (m_observers[eventTypeId].Contains(p_observer))
                {
                    m_observers[eventTypeId].Remove(p_observer);
                }
            }
        }

        public static void UnregisterFromAll(IObserver p_observer)
        {
            Instance.UnregisterFromAllInternal(p_observer);
        }

        private void UnregisterFromAllInternal(IObserver p_observer)
        {
            if (p_observer == null)
            {
                return;
            }

            for (int i = 0; i < m_observers.Count; i++)
            {
                if (!m_observers[i].Contains(p_observer))
                {
                    continue;
                }

                m_observers[i].Remove(p_observer);
            }
        }

        public static void Dispatch<T>(T p_gameEvent, object p_data) where T : IComparable
        {
            Instance.DispatchInternal(p_gameEvent, p_data);
        }

        private void DispatchInternal<T>(T p_gameEvent, object p_data) where T : IComparable
        {
            m_gameEvents.Enqueue(new GameEventData(p_gameEvent, p_data));

            if (!isProcessingEvent)
            {
                DispatchEvents();
            }
        }

        private void DispatchEvents()
        {
            isProcessingEvent = true;

            while (m_gameEvents.Count > 0)
            {
                GameEventData gameEventData = m_gameEvents.Dequeue();

                int eventTypeId = gameEventData.gameEvent.GetType().GetHashCode();

                if (m_observers.TryGetValue(eventTypeId, out var observerList))
                {
                    for (int i = 0; i < observerList.Count; i++)
                    {
                        observerList[i].HandleEvent(gameEventData.gameEvent, gameEventData.data);
                    }
                }
            }

            isProcessingEvent = false;
        }
    }
}

