using UnityEngine;
using UnityEngine.Events;

// Payload Player: game winner
public class GameOverEvent : UnityEvent <Player>
{
    public const int OP_CODE = 40;
}


