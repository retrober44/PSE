using UnityEngine.Events;

// Payload string: error message
public class InvalidRequestEvent : UnityEvent<string>
{
    public const int OP_CODE = 311;
}



