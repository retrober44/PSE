using UnityEngine.Events;

// Payload string: error message
public class InvalidMoveEvent : UnityEvent<string>
{
    public const int OP_CODE = 320;
}



