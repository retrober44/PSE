using UnityEngine;

public class GameEnums : MonoBehaviour
{
    public enum State
    {
        myTurn,
        opponentTurn,
        pause,          //luckywheel spinn, item move actions, ...
        gameEnd,
        error
    }

}
