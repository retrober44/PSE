using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerPrototypeSceneTest : MonoBehaviour
{
    /// <summary>
    /// Very simple test function that should Debug.Log "Hello world", returned from init().
    /// Proves that getInstance() works.
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = GameManager.getInstance();
        gameManager.Init();
        Debug.Log("(Test): GameManager found InputManager " + gameManager.getManager(GameManager.SubManagerTypes.INPUT));

        TurnManager turnManager = (TurnManager)gameManager.getManager(GameManager.SubManagerTypes.TURN);
        turnManager.startGame();
    }
}
