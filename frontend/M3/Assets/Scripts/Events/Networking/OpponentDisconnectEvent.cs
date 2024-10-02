using UnityEngine.Events;

// Payload string: error message
public class OpponentDisconnectEvent : UnityEvent<string>
{
    public const int OP_CODE = 300;
}



