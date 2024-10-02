using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class StateChangeEvent : UnityEvent<StateChangeEventData> { }

public class StateChangeEventData
{
    GameEnums.State newState;
    GameEnums.State prevState;

    public StateChangeEventData(GameEnums.State newState, GameEnums.State prevState)
    {
        this.newState = newState;
        this.prevState = prevState;
    }

}