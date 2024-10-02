using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : ManagerComponent
{
    public Team playerTeam = Team.Ally; // TODO DYNAMIC ASSIGNMENT OF THIS
    
    public enum SubManagerTypes
    {
        INPUT,
        TURN
    }
    
    private Dictionary<SubManagerTypes, ManagerComponent> subManagers = new Dictionary<SubManagerTypes,ManagerComponent>();
    
    /// <summary>
    /// Returns the GameManager instance on the GameManager component in the hierarchy if available
    /// or throws exception.
    /// </summary>
    /// <returns>GameManager</returns>
    public static GameManager getInstance()
    {
        GameObject gameManagerGO = GameObject.Find("GameManager");
        if (gameManagerGO)
        {
            return gameManagerGO.GetComponent<GameManager>();
        }

        Exception noGameManager = new Exception("[GameManager.getInstance()]: No GameManager hierarchy object found. No instance avilable.");
        Debug.LogException(noGameManager);
        throw new Exception(noGameManager.Message);
    }

    /// <summary>
    /// Queries a subManager (i.e. Input, Board) by type. Will throw an exception if the qureried manager is not
    /// avilable.
    /// </summary>
    /// <param name="type">Manager to query, SubManagerTypes.</param>
    /// <returns>ManagerComponent inheritant</returns>
    /// <exception cref="Exception">NoManagerOfType if queried manager is unvailable</exception>
    public ManagerComponent getManager(SubManagerTypes type)
    {
       if (subManagers.ContainsKey(type))
       {
           ManagerComponent queriedManagerComponent = subManagers[type];
           if (queriedManagerComponent)
           {
               return queriedManagerComponent;
           }
       }
       
       // Is thrown if nothing is returned.
       throw new Exception("NoManagerOfType");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Finds submanagers and adds them into the dictionary.
    /// Logs errors if a submanager is not found.
    /// </summary>
    /// <returns>string Hello world</returns>
    public override void Init()
    {
        ManagerComponent turnManager = gameObject.GetComponent<TurnManager>();
        if (turnManager)
        {
            turnManager.Init();
            subManagers.Add(SubManagerTypes.TURN, turnManager); 
        }
        else
        {
            Debug.LogError(gameObject.name + " has no TurnManager attached!");
        }

        ManagerComponent inputManager = gameObject.GetComponent<InputManager>();
        if (inputManager)
        {
            inputManager.Init();
            subManagers.Add(SubManagerTypes.INPUT, inputManager); 
        }
        else
        {
            Debug.LogError(gameObject.name + " has no InputManager attached!");
        }
    }
}
