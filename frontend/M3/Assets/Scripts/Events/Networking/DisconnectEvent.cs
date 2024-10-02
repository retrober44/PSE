using UnityEngine.Events;

// Payload string: error message
public class DisconnectEvent : UnityEvent<string>
{
    public const int OP_CODE = 2;
}



