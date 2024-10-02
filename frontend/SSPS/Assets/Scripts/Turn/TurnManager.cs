using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurnManager : ManagerComponent
{
    Team currentTeam = Team.Ally;
    /**
     * Counter max.
     */
    public const float MaxTurnTime = 20;
    /**
     * Counter variable. Decreased in Update.
     */
    private float _timeLeft;

    private bool _gameInProgress;

    private GameManager _gameManager;

    // Start is called before the first frame update
    public override void Init()
    {
        _gameManager = GameManager.getInstance();
        _timeLeft = MaxTurnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameInProgress)
        {
            // Countdown!
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0)
            {
                // Turn is over! 
                // todo integrate networking with this!
                if (currentTeam == Team.Ally)
                {
                    currentTeam = Team.Enemy;
                }
                else
                {
                    currentTeam = Team.Ally;
                }

                _timeLeft = MaxTurnTime;
            }
        }
    }

    /// <summary>
    /// Debug method. Draws current time and team in the center of the screen.
    /// </summary>
    void OnDrawGizmos()
    {
        string teamLabel = currentTeam == Team.Ally ? "WHITE" : "BLACK";
        
        // We're not drawing this on devices.
        if (Application.isEditor)
        {
            Handles.Label(transform.position, teamLabel + ": " + _timeLeft.ToString("F0") + " s left");
        }
    }

    /// <summary>
    /// Called when turns and countdowns should start.
    /// </summary>
    public void startGame()
    {
        _timeLeft = MaxTurnTime;
        _gameInProgress = true;
    }

    /// <summary>
    /// Returns true if the local player may manipulate game state.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsLocalPlayersTurn()
    {
        return currentTeam == _gameManager.playerTeam;
    }
}
