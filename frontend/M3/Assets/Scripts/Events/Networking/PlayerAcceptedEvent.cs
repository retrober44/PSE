using UnityEngine;
using UnityEngine.Events;

public class PlayerAcceptedEvent : UnityEvent<string>
{
    public const int OP_CODE = 1;
}

