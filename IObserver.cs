using System;

namespace Eventing.EventSystem
{
    public interface IObserver
    {
        void HandleEvent(IComparable p_gameEvent, object p_data);
    }
}
