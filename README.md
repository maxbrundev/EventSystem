# EventSystem
Unity Event System

## Usage
```c#
public enum EGameplayEvent
{
    Save,
    MissionCompleted,
    ScoreUpdate
    ...
}

private readonly Type[] EVENTS_TO_REGISTER_TO = new Type[] { typeof(EGameplayEvent), ...};

EventSystem.Register(this, EVENTS_TO_REGISTER_TO);

EventSystem.Unregister(this, EVENTS_TO_REGISTER_TO);

EventSystem.Dispatch(EGameplayEvent.MissionCompleted, missionID);

public void HandleEvent(IComparable p_gameEvent, object p_data)
{
    if (p_gameEvent is EGameplayEvent)
    {
        switch (p_gameEvent)
        {
            case EGameplayEvent.MissionCompleted:
                //Execute Code...
                break;
            case EGameplayEvent.ScoreUpdate:
                 //Execute Code...
                break;
            default:
                break;
        }
    }
}
```