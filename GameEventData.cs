using System;

namespace Eventing.EventSystem
{
    struct GameEventData
    {
        public IComparable gameEvent;
        public object data;

        public GameEventData(IComparable p_gameEvent, object p_data)
        {
            gameEvent = p_gameEvent;
            data = p_data;
        }
    }
}